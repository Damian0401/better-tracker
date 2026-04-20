using BetterTracker.Common.Results;
using BetterTracker.Contracts;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.JobApplications.Commands;

public static class DeleteJobApplication
{
    public static async Task<Result> HandleAsync(
        DeleteJobApplicationRequest request,
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

        var existingTags = await jobApplicationRepository.ListTagsByJobApplicationIdAsync(request.Id, cancellationToken);
        var candidateTagIds = existingTags
            .Select(x => x.TagId)
            .Distinct()
            .ToList();

        jobApplicationRepository.Remove(jobApplication);

        await tagRepository.RemoveOrphanedByUserIdAsync(
            userId,
            candidateTagIds,
            jobApplication.Id,
            cancellationToken);

        await jobApplicationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
