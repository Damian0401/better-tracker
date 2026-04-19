using BetterTracker.Common.Results;
using BetterTracker.Contracts;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.JobApplications.Commands;

public static class DeleteJobApplicationComment
{
    public static async Task<Result> HandleAsync(
        DeleteJobApplicationCommentRequest request,
        Guid userId,
        IJobApplicationRepository jobApplicationRepository,
        CancellationToken cancellationToken)
    {
        var comment = await jobApplicationRepository.GetCommentByIdAsync(request.Id, cancellationToken);
        if (comment is null)
        {
            return Result.Failure("Comment not found");
        }

        var jobApplication = await jobApplicationRepository.GetByIdAsync(comment.JobApplicationId, cancellationToken);
        if (jobApplication is null || jobApplication.UserId != userId)
        {
            return Result.Failure("Comment not found");
        }

        jobApplicationRepository.RemoveComment(comment);
        await jobApplicationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
