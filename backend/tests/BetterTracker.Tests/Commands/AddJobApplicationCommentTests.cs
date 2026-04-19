using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Commands;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;
using FluentAssertions;
using NSubstitute;

namespace BetterTracker.Tests.Commands;

public class AddJobApplicationCommentTests
{
    private readonly IJobApplicationRepository jobApplicationRepository;

    public AddJobApplicationCommentTests()
    {
        this.jobApplicationRepository = Substitute.For<IJobApplicationRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldAddComment_WhenJobApplicationExistsForUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var jobApplicationId = Guid.NewGuid();

        var request = new AddJobApplicationCommentRequest
        {
            JobApplicationId = jobApplicationId,
            Content = "Comment content",
        };

        this.jobApplicationRepository.GetByIdAsync(jobApplicationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(new JobApplicationEntity
            {
                Id = jobApplicationId,
                UserId = userId,
                Title = "T",
                JobTitle = "JT",
                CompanyName = "C",
                WorkType = WorkType.Remote,
                CurrentStatus = JobApplicationStatus.Applied,
            }));

        // Act
        var result = await AddJobApplicationComment.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        this.jobApplicationRepository.Received(1).AddComment(Arg.Any<JobApplicationCommentEntity>());
        await this.jobApplicationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
