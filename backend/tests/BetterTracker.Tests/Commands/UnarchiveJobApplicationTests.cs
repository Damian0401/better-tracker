using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Commands;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;
using FluentAssertions;
using NSubstitute;

namespace BetterTracker.Tests.Commands;

public class UnarchiveJobApplicationTests
{
    private readonly IJobApplicationRepository jobApplicationRepository;

    public UnarchiveJobApplicationTests()
    {
        this.jobApplicationRepository = Substitute.For<IJobApplicationRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenJobApplicationDoesNotExist()
    {
        // Arrange
        var request = new UnarchiveJobApplicationRequest
        {
            Id = Guid.NewGuid(),
        };

        this.jobApplicationRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(null));

        // Act
        var result = await UnarchiveJobApplication.HandleAsync(
            request,
            Guid.NewGuid(),
            this.jobApplicationRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        this.jobApplicationRepository.DidNotReceive().Update(Arg.Any<JobApplicationEntity>());
        await this.jobApplicationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldUnarchiveJobApplication_WhenOwnedByUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UnarchiveJobApplicationRequest
        {
            Id = Guid.NewGuid(),
        };

        var jobApplication = new JobApplicationEntity
        {
            Id = request.Id,
            UserId = userId,
            JobTitle = "Job",
            CompanyName = "Company",
            WorkType = WorkType.Remote,
            CurrentStatus = JobApplicationStatus.Applied,
            IsArchived = true,
        };

        this.jobApplicationRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(jobApplication));

        // Act
        var result = await UnarchiveJobApplication.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        jobApplication.IsArchived.Should().BeFalse();
        this.jobApplicationRepository.Received(1).Update(Arg.Is(jobApplication));
        await this.jobApplicationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccessWithoutSaving_WhenAlreadyActive()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UnarchiveJobApplicationRequest
        {
            Id = Guid.NewGuid(),
        };

        var jobApplication = new JobApplicationEntity
        {
            Id = request.Id,
            UserId = userId,
            JobTitle = "Job",
            CompanyName = "Company",
            WorkType = WorkType.Remote,
            CurrentStatus = JobApplicationStatus.Applied,
            IsArchived = false,
        };

        this.jobApplicationRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(jobApplication));

        // Act
        var result = await UnarchiveJobApplication.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        this.jobApplicationRepository.DidNotReceive().Update(Arg.Any<JobApplicationEntity>());
        await this.jobApplicationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
