using BetterTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Data.Repositories;

public sealed class JobApplicationRepository : IJobApplicationRepository
{
    private readonly AppDbContext dbContext;

    public JobApplicationRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<JobApplicationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.dbContext.JobApplications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<JobApplicationCommentEntity?> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.dbContext.JobApplicationComments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Add(JobApplicationEntity jobApplication)
    {
        this.dbContext.JobApplications.Add(jobApplication);
    }

    public void Remove(JobApplicationEntity jobApplication)
    {
        this.dbContext.JobApplications.Remove(jobApplication);
    }

    public void AddComment(JobApplicationCommentEntity comment)
    {
        this.dbContext.JobApplicationComments.Add(comment);
    }

    public void RemoveComment(JobApplicationCommentEntity comment)
    {
        this.dbContext.JobApplicationComments.Remove(comment);
    }

    public void Update(JobApplicationEntity jobApplication)
    {
        this.dbContext.JobApplications.Update(jobApplication);
    }

    public void AddSalary(JobApplicationSalaryEntity salary)
    {
        this.dbContext.JobApplicationSalaries.Add(salary);
    }

    public void RemoveSalaries(IEnumerable<JobApplicationSalaryEntity> salaries)
    {
        this.dbContext.JobApplicationSalaries.RemoveRange(salaries);
    }

    public void AddStatusHistory(JobApplicationStatusHistoryEntity statusHistory)
    {
        this.dbContext.JobApplicationStatusHistory.Add(statusHistory);
    }

    public void AddTag(JobApplicationTagEntity jobApplicationTag)
    {
        this.dbContext.JobApplicationTags.Add(jobApplicationTag);
    }

    public void RemoveTags(IEnumerable<JobApplicationTagEntity> tags)
    {
        this.dbContext.JobApplicationTags.RemoveRange(tags);
    }

    public async Task<IReadOnlyList<JobApplicationSalaryEntity>> ListSalariesByJobApplicationIdAsync(
        Guid jobApplicationId,
        CancellationToken cancellationToken = default)
    {
        return await this.dbContext.JobApplicationSalaries
            .Where(x => x.JobApplicationId == jobApplicationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JobApplicationTagEntity>> ListTagsByJobApplicationIdAsync(
        Guid jobApplicationId,
        CancellationToken cancellationToken = default)
    {
        return await this.dbContext.JobApplicationTags
            .Where(x => x.JobApplicationId == jobApplicationId)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.dbContext.SaveChangesAsync(cancellationToken);
    }
}
