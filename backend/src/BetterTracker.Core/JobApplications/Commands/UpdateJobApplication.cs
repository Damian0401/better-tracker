using BetterTracker.Common.Results;
using BetterTracker.Contracts;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.JobApplications.Commands;

public static class UpdateJobApplication
{
    public static async Task<Result> HandleAsync(
        UpdateJobApplicationRequest request,
        Guid userId,
        IJobApplicationRepository jobApplicationRepository,
        ITagRepository tagRepository,
        CancellationToken cancellationToken)
    {
        var jobApplication = await jobApplicationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (jobApplication is null || jobApplication.UserId != userId)
        {
            return Result.Failure("Job application not found");
        }

        var validationResult = ValidateRequest(request);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var previousStatus = jobApplication.CurrentStatus;

        UpdateJobApplicationFields(jobApplication, request);

        jobApplicationRepository.Update(jobApplication);

        var existingSalaries = await jobApplicationRepository.ListSalariesByJobApplicationIdAsync(request.Id, cancellationToken);
        SyncSalaries(request, jobApplication, existingSalaries, jobApplicationRepository);

        AddStatusHistoryIfChanged(previousStatus, jobApplication, jobApplicationRepository);

        var existingTags = await jobApplicationRepository.ListTagsByJobApplicationIdAsync(request.Id, cancellationToken);
        await SyncTagsAsync(
            request,
            userId,
            jobApplication,
            existingTags,
            jobApplicationRepository,
            tagRepository,
            cancellationToken);

        await jobApplicationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static Result ValidateRequest(UpdateJobApplicationRequest request)
    {
        if (!Enum.IsDefined((WorkType)request.WorkType))
        {
            return Result.Failure("Invalid work type");
        }

        if (!Enum.IsDefined((JobApplicationStatus)request.CurrentStatus))
        {
            return Result.Failure("Invalid current status");
        }

        var salaryDtos = request.Salaries ?? [];
        var duplicateSalaryTypes = salaryDtos
            .GroupBy(x => x.SalaryType)
            .FirstOrDefault(g => g.Count() > 1);
        if (duplicateSalaryTypes is not null)
        {
            return Result.Failure("Salary types must be unique per application");
        }

        var invalidSalaryType = salaryDtos
            .FirstOrDefault(x => !Enum.IsDefined((SalaryType)x.SalaryType));
        if (invalidSalaryType is not null)
        {
            return Result.Failure("Invalid salary type");
        }

        return Result.Success();
    }

    private static void UpdateJobApplicationFields(JobApplicationEntity jobApplication, UpdateJobApplicationRequest request)
    {
        jobApplication.Title = request.Title.Trim();
        jobApplication.JobTitle = request.JobTitle.Trim();
        jobApplication.Description = NormalizeNullable(request.Description);
        jobApplication.CompanyName = request.CompanyName.Trim();
        jobApplication.Requirements = NormalizeNullable(request.Requirements);
        jobApplication.Benefits = NormalizeNullable(request.Benefits);
        jobApplication.Link = NormalizeNullable(request.Link);
        jobApplication.Technologies = NormalizeNullable(request.Technologies);
        jobApplication.Experience = NormalizeNullable(request.Experience);
        jobApplication.WorkType = (WorkType)request.WorkType;
        jobApplication.CurrentStatus = (JobApplicationStatus)request.CurrentStatus;
    }

    private static void SyncSalaries(
        UpdateJobApplicationRequest request,
        JobApplicationEntity jobApplication,
        IReadOnlyList<JobApplicationSalaryEntity> existingSalaries,
        IJobApplicationRepository jobApplicationRepository)
    {
        var requestedSalariesByType = (request.Salaries ?? [])
            .ToDictionary(x => (SalaryType)x.SalaryType);
        var existingSalariesByType = existingSalaries
            .ToDictionary(x => x.SalaryType);

        foreach (var (salaryType, salaryDto) in requestedSalariesByType)
        {
            if (existingSalariesByType.TryGetValue(salaryType, out var existingSalary))
            {
                existingSalary.SalaryPost = salaryDto.SalaryPost;
                existingSalary.SalaryCandidate = salaryDto.SalaryCandidate;
                existingSalary.Currency = NormalizeNullable(salaryDto.Currency)?.ToUpperInvariant();
                continue;
            }

            jobApplicationRepository.AddSalary(new JobApplicationSalaryEntity
            {
                JobApplicationId = jobApplication.Id,
                SalaryType = salaryType,
                SalaryPost = salaryDto.SalaryPost,
                SalaryCandidate = salaryDto.SalaryCandidate,
                Currency = NormalizeNullable(salaryDto.Currency)?.ToUpperInvariant(),
            });
        }

        var salariesToRemove = existingSalaries
            .Where(x => !requestedSalariesByType.ContainsKey(x.SalaryType))
            .ToList();
        if (salariesToRemove.Count > 0)
        {
            jobApplicationRepository.RemoveSalaries(salariesToRemove);
        }
    }

    private static void AddStatusHistoryIfChanged(
        JobApplicationStatus previousStatus,
        JobApplicationEntity jobApplication,
        IJobApplicationRepository jobApplicationRepository)
    {
        if (previousStatus == jobApplication.CurrentStatus)
        {
            return;
        }

        jobApplicationRepository.AddStatusHistory(new JobApplicationStatusHistoryEntity
        {
            JobApplicationId = jobApplication.Id,
            PreviousStatus = previousStatus,
            NewStatus = jobApplication.CurrentStatus,
        });
    }

    private static async Task SyncTagsAsync(
        UpdateJobApplicationRequest request,
        Guid userId,
        JobApplicationEntity jobApplication,
        IReadOnlyList<JobApplicationTagEntity> existingTags,
        IJobApplicationRepository jobApplicationRepository,
        ITagRepository tagRepository,
        CancellationToken cancellationToken)
    {
        var normalizedTagNames = (request.Tags ?? [])
            .Select(NormalizeNullable)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var desiredTagIds = new HashSet<Guid>();
        foreach (var tagName in normalizedTagNames)
        {
            var tag = await tagRepository.GetByUserIdAndNameAsync(userId, tagName, cancellationToken);
            if (tag is null)
            {
                tag = new TagEntity
                {
                    UserId = userId,
                    Name = tagName,
                };

                tagRepository.Add(tag);
            }

            desiredTagIds.Add(tag.Id);
        }

        var existingTagIds = existingTags
            .Select(x => x.TagId)
            .ToHashSet();

        var tagsToRemove = existingTags
            .Where(x => !desiredTagIds.Contains(x.TagId))
            .ToList();
        if (tagsToRemove.Count > 0)
        {
            jobApplicationRepository.RemoveTags(tagsToRemove);
        }

        foreach (var tagId in desiredTagIds)
        {
            if (existingTagIds.Contains(tagId))
            {
                continue;
            }

            jobApplicationRepository.AddTag(new JobApplicationTagEntity
            {
                JobApplicationId = jobApplication.Id,
                TagId = tagId,
            });
        }
    }

    private static string? NormalizeNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
