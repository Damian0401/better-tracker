using BetterTracker.Contracts;
using BetterTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Core.JobApplications.Queries;

public static class GetJobApplicationById
{
    public static async ValueTask<GetJobApplicationByIdResponse?> HandleAsync(
        Guid id,
        Guid userId,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var jobApplication = await dbContext.JobApplications
            .Where(x => x.Id == id && x.UserId == userId)
            .Select(x => new GetJobApplicationByIdDto
            {
                Id = x.Id,
                JobTitle = x.JobTitle,
                Description = x.Description,
                CompanyName = x.CompanyName,
                Requirements = x.Requirements,
                Benefits = x.Benefits,
                Link = x.Link,
                Technologies = x.Technologies,
                Experience = x.Experience,
                WorkType = (int)x.WorkType,
                CurrentStatus = (int)x.CurrentStatus,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Tags = new List<string>(),
                Salaries = new List<GetJobApplicationByIdSalaryDto>(),
                StatusHistory = new List<GetJobApplicationByIdStatusHistoryDto>(),
                Comments = new List<GetJobApplicationByIdCommentDto>(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (jobApplication is null)
        {
            return null;
        }

        var tags = await dbContext.JobApplicationTags
            .Where(x => x.JobApplicationId == id)
            .Join(
                dbContext.Tags,
                x => x.TagId,
                tag => tag.Id,
                (_, tag) => tag.Name)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var salaries = await dbContext.JobApplicationSalaries
            .Where(x => x.JobApplicationId == id)
            .OrderBy(x => x.SalaryType)
            .Select(x => new GetJobApplicationByIdSalaryDto
            {
                SalaryType = (int)x.SalaryType,
                OfferFrom = x.OfferFrom,
                OfferTo = x.OfferTo,
                ExpectedFrom = x.ExpectedFrom,
                ExpectedTo = x.ExpectedTo,
                Currency = x.Currency,
            })
            .ToListAsync(cancellationToken);

        var statusHistory = await dbContext.JobApplicationStatusHistory
            .Where(x => x.JobApplicationId == id)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new GetJobApplicationByIdStatusHistoryDto
            {
                PreviousStatus = x.PreviousStatus.HasValue ? (int)x.PreviousStatus.Value : null,
                NewStatus = (int)x.NewStatus,
                ChangedAt = x.CreatedAt,
            })
            .ToListAsync(cancellationToken);

        var comments = await dbContext.JobApplicationComments
            .Where(x => x.JobApplicationId == id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetJobApplicationByIdCommentDto
            {
                Id = x.Id,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
            .ToListAsync(cancellationToken);

        return new GetJobApplicationByIdResponse
        {
            JobApplication = jobApplication with
            {
                Tags = tags,
                Salaries = salaries,
                StatusHistory = statusHistory,
                Comments = comments,
            },
        };
    }
}
