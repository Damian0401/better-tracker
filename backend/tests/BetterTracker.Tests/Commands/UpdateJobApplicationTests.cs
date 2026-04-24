using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Commands;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;
using FluentAssertions;
using NSubstitute;

namespace BetterTracker.Tests.Commands;

public class UpdateJobApplicationTests
{
    private readonly IJobApplicationRepository jobApplicationRepository;
    private readonly ITagRepository tagRepository;

    public UpdateJobApplicationTests()
    {
        this.jobApplicationRepository = Substitute.For<IJobApplicationRepository>();
        this.tagRepository = Substitute.For<ITagRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenJobApplicationDoesNotExist()
    {
        // Arrange
        var request = new UpdateJobApplicationRequest
        {
            Id = Guid.NewGuid(),
            JobTitle = "Job Title",
            CompanyName = "Company",
            WorkType = 1,
            CurrentStatus = 0,
        };

        this.jobApplicationRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(null));

        // Act
        var result = await UpdateJobApplication.HandleAsync(
            request,
            Guid.NewGuid(),
            this.jobApplicationRepository,
            this.tagRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        this.jobApplicationRepository.DidNotReceive().Update(Arg.Any<JobApplicationEntity>());
        await this.jobApplicationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateJobApplicationAndSave_WhenRequestIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdateJobApplicationRequest
        {
            Id = Guid.NewGuid(),
            JobTitle = "New Job Title",
            CompanyName = "New Company",
            WorkType = 2,
            CurrentStatus = 1,
            Salaries =
            [
                new UpdateJobApplicationSalaryDto
                {
                    SalaryType = 0,
                    OfferFrom = 10000,
                    OfferTo = 12000,
                    ExpectedFrom = 13000,
                    ExpectedTo = 14000,
                    Currency = "usd",
                }
            ],
            Tags = ["Urgent"],
        };

        var existing = new JobApplicationEntity
        {
            Id = request.Id,
            UserId = userId,
            JobTitle = "Old Job Title",
            CompanyName = "Old Company",
            WorkType = WorkType.Remote,
            CurrentStatus = JobApplicationStatus.Applied,
        };

        this.jobApplicationRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(existing));
        this.jobApplicationRepository.ListSalariesByJobApplicationIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<JobApplicationSalaryEntity>>([]));
        this.jobApplicationRepository.ListTagsByJobApplicationIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<JobApplicationTagEntity>>([]));

        // Act
        var result = await UpdateJobApplication.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            this.tagRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        this.jobApplicationRepository.Received(1).Update(Arg.Any<JobApplicationEntity>());
        this.jobApplicationRepository.Received(1).AddSalary(Arg.Any<JobApplicationSalaryEntity>());
        this.jobApplicationRepository.Received(1).AddStatusHistory(Arg.Any<JobApplicationStatusHistoryEntity>());
        this.jobApplicationRepository.Received(1).AddTag(Arg.Any<JobApplicationTagEntity>());
        await this.jobApplicationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateExistingSalaryAndPreserveExistingTag_WhenTheyAlreadyExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var jobApplicationId = Guid.NewGuid();
        var existingTagId = Guid.NewGuid();

        var request = new UpdateJobApplicationRequest
        {
            Id = jobApplicationId,
            JobTitle = "Updated Job",
            CompanyName = "Updated Company",
            WorkType = 1,
            CurrentStatus = 0,
            Salaries =
            [
                new UpdateJobApplicationSalaryDto
                {
                    SalaryType = 0,
                    OfferFrom = 15000,
                    OfferTo = 17000,
                    ExpectedFrom = 18000,
                    ExpectedTo = 19000,
                    Currency = "eur",
                }
            ],
            Tags = ["Urgent"],
        };

        var existingJobApplication = new JobApplicationEntity
        {
            Id = jobApplicationId,
            UserId = userId,
            JobTitle = "Old",
            CompanyName = "Old",
            WorkType = WorkType.Remote,
            CurrentStatus = JobApplicationStatus.Applied,
        };

        var existingSalary = new JobApplicationSalaryEntity
        {
            Id = Guid.NewGuid(),
            JobApplicationId = jobApplicationId,
            SalaryType = SalaryType.B2B,
            OfferFrom = 10000,
            OfferTo = 11000,
            ExpectedFrom = 12000,
            ExpectedTo = 13000,
            Currency = "USD",
        };

        var existingTag = new TagEntity
        {
            Id = existingTagId,
            UserId = userId,
            Name = "Urgent",
        };

        var existingTagLink = new JobApplicationTagEntity
        {
            Id = Guid.NewGuid(),
            JobApplicationId = jobApplicationId,
            TagId = existingTagId,
        };

        this.jobApplicationRepository.GetByIdAsync(jobApplicationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(existingJobApplication));
        this.jobApplicationRepository.ListSalariesByJobApplicationIdAsync(jobApplicationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<JobApplicationSalaryEntity>>([existingSalary]));
        this.jobApplicationRepository.ListTagsByJobApplicationIdAsync(jobApplicationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<JobApplicationTagEntity>>([existingTagLink]));
        this.tagRepository.GetByUserIdAndNameAsync(userId, "Urgent", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TagEntity?>(existingTag));

        // Act
        var result = await UpdateJobApplication.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            this.tagRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingSalary.OfferFrom.Should().Be(15000);
        existingSalary.OfferTo.Should().Be(17000);
        existingSalary.ExpectedFrom.Should().Be(18000);
        existingSalary.ExpectedTo.Should().Be(19000);
        existingSalary.Currency.Should().Be("EUR");
        this.jobApplicationRepository.DidNotReceive().AddSalary(Arg.Any<JobApplicationSalaryEntity>());
        this.jobApplicationRepository.DidNotReceive().RemoveSalaries(Arg.Any<IEnumerable<JobApplicationSalaryEntity>>());
        this.jobApplicationRepository.DidNotReceive().AddTag(Arg.Any<JobApplicationTagEntity>());
        this.jobApplicationRepository.DidNotReceive().RemoveTags(Arg.Any<IEnumerable<JobApplicationTagEntity>>());
        await this.tagRepository.DidNotReceive().RemoveOrphanedByUserIdAsync(
            Arg.Any<Guid>(),
            Arg.Any<IReadOnlyCollection<Guid>>(),
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveOrphanedTags_WhenTagIsDetachedFromJobApplication()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var jobApplicationId = Guid.NewGuid();
        var removedTagId = Guid.NewGuid();
        var request = new UpdateJobApplicationRequest
        {
            Id = jobApplicationId,
            JobTitle = "Updated Job",
            CompanyName = "Updated Company",
            WorkType = 1,
            CurrentStatus = 0,
            Tags = [],
        };

        var existingJobApplication = new JobApplicationEntity
        {
            Id = jobApplicationId,
            UserId = userId,
            JobTitle = "Old",
            CompanyName = "Old",
            WorkType = WorkType.Remote,
            CurrentStatus = JobApplicationStatus.Applied,
        };

        var existingTagLink = new JobApplicationTagEntity
        {
            Id = Guid.NewGuid(),
            JobApplicationId = jobApplicationId,
            TagId = removedTagId,
        };

        this.jobApplicationRepository.GetByIdAsync(jobApplicationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(existingJobApplication));
        this.jobApplicationRepository.ListSalariesByJobApplicationIdAsync(jobApplicationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<JobApplicationSalaryEntity>>([]));
        this.jobApplicationRepository.ListTagsByJobApplicationIdAsync(jobApplicationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<JobApplicationTagEntity>>([existingTagLink]));

        // Act
        var result = await UpdateJobApplication.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            this.tagRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        this.jobApplicationRepository.Received(1).RemoveTags(Arg.Any<IEnumerable<JobApplicationTagEntity>>());
        await this.tagRepository.Received(1).RemoveOrphanedByUserIdAsync(
            userId,
            Arg.Is<IReadOnlyCollection<Guid>>(x => x.Count == 1 && x.Contains(removedTagId)),
            jobApplicationId,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateArchivedJobApplication_WhenOwnedByUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdateJobApplicationRequest
        {
            Id = Guid.NewGuid(),
            JobTitle = "Updated Job Title",
            CompanyName = "Updated Company",
            WorkType = 1,
            CurrentStatus = 0,
        };

        var archivedJobApplication = new JobApplicationEntity
        {
            Id = request.Id,
            UserId = userId,
            JobTitle = "Old Job Title",
            CompanyName = "Old Company",
            WorkType = WorkType.Remote,
            CurrentStatus = JobApplicationStatus.Applied,
            IsArchived = true,
        };

        this.jobApplicationRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<JobApplicationEntity?>(archivedJobApplication));
        this.jobApplicationRepository.ListSalariesByJobApplicationIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<JobApplicationSalaryEntity>>([]));
        this.jobApplicationRepository.ListTagsByJobApplicationIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<JobApplicationTagEntity>>([]));

        // Act
        var result = await UpdateJobApplication.HandleAsync(
            request,
            userId,
            this.jobApplicationRepository,
            this.tagRepository,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        archivedJobApplication.JobTitle.Should().Be("Updated Job Title");
        archivedJobApplication.CompanyName.Should().Be("Updated Company");
        archivedJobApplication.IsArchived.Should().BeTrue();
        this.jobApplicationRepository.Received(1).Update(Arg.Is(archivedJobApplication));
        await this.jobApplicationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
