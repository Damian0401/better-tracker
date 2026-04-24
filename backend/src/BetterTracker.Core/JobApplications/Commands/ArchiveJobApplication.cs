using BetterTracker.Common.Results;
using BetterTracker.Contracts;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.JobApplications.Commands;

public static class ArchiveJobApplication
{
    public static async Task<Result> HandleAsync(
        ArchiveJobApplicationRequest request,
        Guid userId,
        IJobApplicationRepository jobApplicationRepository,
        CancellationToken cancellationToken)
    {
        var jobApplication = await jobApplicationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (jobApplication is null || jobApplication.UserId != userId)
        {
            return Result.Failure("Job application not found");
        }

        if (jobApplication.IsArchived)
        {
            return Result.Success();
        }

        jobApplication.IsArchived = true;
        jobApplicationRepository.Update(jobApplication);
        await jobApplicationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
