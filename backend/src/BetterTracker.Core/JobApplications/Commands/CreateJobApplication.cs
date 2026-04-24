using BetterTracker.Common.Results;
using BetterTracker.Contracts;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.JobApplications.Commands;

public static class CreateJobApplication
{
    public static async Task<Result<CreateJobApplicationResponse>> HandleAsync(
        CreateJobApplicationRequest request,
        Guid userId,
        IJobApplicationRepository jobApplicationRepository,
        ITagRepository tagRepository,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined((WorkType)request.WorkType))
        {
            return Result<CreateJobApplicationResponse>.Failure("Invalid work type");
        }

        if (!Enum.IsDefined((JobApplicationStatus)request.CurrentStatus))
        {
            return Result<CreateJobApplicationResponse>.Failure("Invalid current status");
        }

        var salaryDtos = request.Salaries ?? [];
        var duplicateSalaryTypes = salaryDtos
            .GroupBy(x => x.SalaryType)
            .FirstOrDefault(g => g.Count() > 1);
        if (duplicateSalaryTypes is not null)
        {
            return Result<CreateJobApplicationResponse>.Failure("Salary types must be unique per application");
        }

        var invalidSalaryType = salaryDtos
            .FirstOrDefault(x => !Enum.IsDefined((SalaryType)x.SalaryType));
        if (invalidSalaryType is not null)
        {
            return Result<CreateJobApplicationResponse>.Failure("Invalid salary type");
        }

        var jobApplication = new JobApplicationEntity
        {
            UserId = userId,
            JobTitle = request.JobTitle.Trim(),
            Description = NormalizeNullable(request.Description),
            CompanyName = request.CompanyName.Trim(),
            Requirements = NormalizeNullable(request.Requirements),
            Benefits = NormalizeNullable(request.Benefits),
            Link = NormalizeNullable(request.Link),
            Technologies = NormalizeNullable(request.Technologies),
            Experience = NormalizeNullable(request.Experience),
            WorkType = (WorkType)request.WorkType,
            CurrentStatus = (JobApplicationStatus)request.CurrentStatus,
        };

        jobApplicationRepository.Add(jobApplication);

        foreach (var salaryDto in salaryDtos)
        {
            var salary = new JobApplicationSalaryEntity
            {
                JobApplicationId = jobApplication.Id,
                SalaryType = (SalaryType)salaryDto.SalaryType,
                OfferFrom = salaryDto.OfferFrom,
                OfferTo = salaryDto.OfferTo,
                ExpectedFrom = salaryDto.ExpectedFrom,
                ExpectedTo = salaryDto.ExpectedTo,
                Currency = NormalizeNullable(salaryDto.Currency)?.ToUpperInvariant(),
            };

            jobApplicationRepository.AddSalary(salary);
        }

        var statusHistory = new JobApplicationStatusHistoryEntity
        {
            JobApplicationId = jobApplication.Id,
            PreviousStatus = null,
            NewStatus = (JobApplicationStatus)request.CurrentStatus,
        };
        jobApplicationRepository.AddStatusHistory(statusHistory);

        var normalizedTags = (request.Tags ?? [])
            .Select(NormalizeNullable)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var tagName in normalizedTags)
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

            jobApplicationRepository.AddTag(new JobApplicationTagEntity
            {
                JobApplicationId = jobApplication.Id,
                TagId = tag.Id,
            });
        }

        await jobApplicationRepository.SaveChangesAsync(cancellationToken);

        return Result<CreateJobApplicationResponse>.Success(new CreateJobApplicationResponse
        {
            Id = jobApplication.Id,
        });
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
