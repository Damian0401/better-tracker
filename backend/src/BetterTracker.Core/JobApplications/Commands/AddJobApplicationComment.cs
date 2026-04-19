using BetterTracker.Common.Results;
using BetterTracker.Contracts;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.JobApplications.Commands;

public static class AddJobApplicationComment
{
    public static async Task<Result<AddJobApplicationCommentResponse>> HandleAsync(
        AddJobApplicationCommentRequest request,
        Guid userId,
        IJobApplicationRepository jobApplicationRepository,
        CancellationToken cancellationToken)
    {
        var jobApplication = await jobApplicationRepository.GetByIdAsync(request.JobApplicationId, cancellationToken);
        if (jobApplication is null || jobApplication.UserId != userId)
        {
            return Result<AddJobApplicationCommentResponse>.Failure("Job application not found");
        }

        var comment = new JobApplicationCommentEntity
        {
            JobApplicationId = request.JobApplicationId,
            Content = request.Content.Trim(),
        };

        jobApplicationRepository.AddComment(comment);
        await jobApplicationRepository.SaveChangesAsync(cancellationToken);

        return Result<AddJobApplicationCommentResponse>.Success(new AddJobApplicationCommentResponse
        {
            Id = comment.Id,
        });
    }
}
