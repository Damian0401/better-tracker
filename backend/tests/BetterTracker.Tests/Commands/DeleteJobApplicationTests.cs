using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Commands;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;
using FluentAssertions;
using NSubstitute;

namespace BetterTracker.Tests.Commands;

public class DeleteJobApplicationTests
{
    private readonly IJobApplicationRepository jobApplicationRepository;
    private readonly ITagRepository tagRepository;

    public DeleteJobApplicationTests()
    {
        this.jobApplicationRepository = Substitute.For<IJobApplicationRepository>();
        this.tagRepository = Substitute.For<ITagRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenJobApplicationDoesNotExist()
    {
        // Arrange
        var request = new DeleteJobApplicationRequest
        {
            Id = Guid.NewGuid(),
        };

        this.jobApplicationRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(null));

        // Act
        var result = await DeleteJobApplication.HandleAsync(
            request,
            Guid.NewGuid(),
            this.jobApplicationRepository,
            this.tagRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        this.jobApplicationRepository.DidNotReceive().Remove(Arg.Any<JobApplicationEntity>());
        await this.tagRepository.DidNotReceive().RemoveOrphanedByUserIdAsync(
            Arg.Any<Guid>(),
            Arg.Any<IReadOnlyCollection<Guid>>(),
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>());
        await this.jobApplicationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteJobApplicationAndCleanupOrphanedTags_WhenOwnedByUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var jobApplicationId = Guid.NewGuid();
        var tagId = Guid.NewGuid();
        var request = new DeleteJobApplicationRequest
        {
            Id = jobApplicationId,
        };

        var jobApplication = new JobApplicationEntity
        {
            Id = jobApplicationId,
            UserId = userId,
            Title = "Title",
            JobTitle = "Job",
            CompanyName = "Company",
            WorkType = WorkType.Remote,
            CurrentStatus = JobApplicationStatus.Applied,
        };

        this.jobApplicationRepository.GetByIdAsync(jobApplicationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(jobApplication));
        this.jobApplicationRepository.ListTagsByJobApplicationIdAsync(jobApplicationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<JobApplicationTagEntity>>(
            [
                new JobApplicationTagEntity
                {
                    Id = Guid.NewGuid(),
                    JobApplicationId = jobApplicationId,
                    TagId = tagId,
                }
            ]));

        // Act
        var result = await DeleteJobApplication.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            this.tagRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        this.jobApplicationRepository.Received(1).Remove(Arg.Is(jobApplication));
        await this.tagRepository.Received(1).RemoveOrphanedByUserIdAsync(
            userId,
            Arg.Is<IReadOnlyCollection<Guid>>(x => x.Count == 1 && x.Contains(tagId)),
            jobApplicationId,
            Arg.Any<CancellationToken>());
        await this.jobApplicationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
