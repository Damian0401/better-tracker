using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Commands;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;
using FluentAssertions;
using NSubstitute;

namespace BetterTracker.Tests.Commands;

public class CreateJobApplicationTests
{
    private readonly IJobApplicationRepository jobApplicationRepository;
    private readonly ITagRepository tagRepository;

    public CreateJobApplicationTests()
    {
        this.jobApplicationRepository = Substitute.For<IJobApplicationRepository>();
        this.tagRepository = Substitute.For<ITagRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldAddJobApplicationAndSave_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateJobApplicationRequest
        {
            Title = "  Apple iOS Developer  ",
            JobTitle = "Mid iOS Developer",
            CompanyName = "Apple",
            WorkType = 1,
            CurrentStatus = 0,
            Tags = ["Urgent", "Startup"],
            Salaries =
            [
                new CreateJobApplicationSalaryDto
                {
                    SalaryType = 0,
                    SalaryPost = 10000,
                    SalaryCandidate = 12000,
                    Currency = "pln",
                }
            ]
        };

        var userId = Guid.NewGuid();

        // Act
        var result = await CreateJobApplication.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            this.tagRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        this.jobApplicationRepository.Received(1).Add(Arg.Any<JobApplicationEntity>());
        this.jobApplicationRepository.Received(1).AddSalary(Arg.Any<JobApplicationSalaryEntity>());
        this.jobApplicationRepository.Received(1).AddStatusHistory(Arg.Any<JobApplicationStatusHistoryEntity>());
        this.jobApplicationRepository.Received(2).AddTag(Arg.Any<JobApplicationTagEntity>());
        this.tagRepository.Received(2).Add(Arg.Any<TagEntity>());
        await this.jobApplicationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenSalaryTypesAreDuplicated()
    {
        // Arrange
        var request = new CreateJobApplicationRequest
        {
            Title = "Title",
            JobTitle = "Job Title",
            CompanyName = "Company",
            WorkType = 1,
            CurrentStatus = 0,
            Salaries =
            [
                new CreateJobApplicationSalaryDto { SalaryType = 0 },
                new CreateJobApplicationSalaryDto { SalaryType = 0 },
            ]
        };

        var userId = Guid.NewGuid();

        // Act
        var result = await CreateJobApplication.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            this.tagRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        this.jobApplicationRepository.DidNotReceive().Add(Arg.Any<JobApplicationEntity>());
        await this.jobApplicationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
