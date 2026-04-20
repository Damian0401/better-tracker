using BetterTracker.Data.Entities;

namespace BetterTracker.Data.Repositories;

public interface IJobApplicationRepository
{
    Task<JobApplicationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JobApplicationCommentEntity?> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Remove(JobApplicationEntity jobApplication);
    void Update(JobApplicationEntity jobApplication);
    void Add(JobApplicationEntity jobApplication);
    void AddComment(JobApplicationCommentEntity comment);
    void RemoveComment(JobApplicationCommentEntity comment);
    void AddSalary(JobApplicationSalaryEntity salary);
    void RemoveSalaries(IEnumerable<JobApplicationSalaryEntity> salaries);
    void AddStatusHistory(JobApplicationStatusHistoryEntity statusHistory);
    void AddTag(JobApplicationTagEntity jobApplicationTag);
    void RemoveTags(IEnumerable<JobApplicationTagEntity> tags);
    Task<IReadOnlyList<JobApplicationSalaryEntity>> ListSalariesByJobApplicationIdAsync(Guid jobApplicationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobApplicationTagEntity>> ListTagsByJobApplicationIdAsync(Guid jobApplicationId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
