using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Commands;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;
using FluentAssertions;
using NSubstitute;

namespace BetterTracker.Tests.Commands;

public class DeleteJobApplicationCommentTests
{
    private readonly IJobApplicationRepository jobApplicationRepository;

    public DeleteJobApplicationCommentTests()
    {
        this.jobApplicationRepository = Substitute.For<IJobApplicationRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveComment_WhenCommentBelongsToUsersJobApplication()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var jobApplicationId = Guid.NewGuid();

        var request = new DeleteJobApplicationCommentRequest
        {
            Id = commentId,
        };

        this.jobApplicationRepository.GetCommentByIdAsync(commentId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationCommentEntity?>(new JobApplicationCommentEntity
            {
                Id = commentId,
                JobApplicationId = jobApplicationId,
                Content = "Comment",
            }));

        this.jobApplicationRepository.GetByIdAsync(jobApplicationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(new JobApplicationEntity
            {
                Id = jobApplicationId,
                UserId = userId,
                JobTitle = "JT",
                CompanyName = "C",
                WorkType = WorkType.Remote,
                CurrentStatus = JobApplicationStatus.Applied,
            }));

        // Act
        var result = await DeleteJobApplicationComment.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        this.jobApplicationRepository.Received(1).RemoveComment(Arg.Any<JobApplicationCommentEntity>());
        await this.jobApplicationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
