using BetterTracker.Contracts;
using BetterTracker.Data;
using BetterTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Core.JobApplications.Queries;

public static class ListJobApplications
{
    private const int DefaultCount = 10;

    public static async ValueTask<ListJobApplicationsResponse> HandleAsync(
        int? count,
        int? skip,
        int? status,
        string? tag,
        int? workType,
        string? search,
        Guid userId,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var take = count ?? DefaultCount;
        var skipCount = skip ?? 0;

        var query = dbContext.JobApplications
            .Where(x => x.UserId == userId)
            .AsQueryable();

        if (status.HasValue && Enum.IsDefined((JobApplicationStatus)status.Value))
        {
            var parsedStatus = (JobApplicationStatus)status.Value;
            query = query.Where(x => x.CurrentStatus == parsedStatus);
        }

        if (workType.HasValue && Enum.IsDefined((WorkType)workType.Value))
        {
            var parsedWorkType = (WorkType)workType.Value;
            query = query.Where(x => x.WorkType == parsedWorkType);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();
            query = query.Where(x =>
                x.Title.Contains(normalizedSearch) ||
                x.JobTitle.Contains(normalizedSearch) ||
                x.CompanyName.Contains(normalizedSearch));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var normalizedTag = tag.Trim();
            query = query.Where(x => dbContext.JobApplicationTags
                .Where(y => y.JobApplicationId == x.Id)
                .Join(
                    dbContext.Tags,
                    y => y.TagId,
                    t => t.Id,
                    (_, t) => t.Name)
                .Any(t => t == normalizedTag));
        }

        var total = await query.CountAsync(cancellationToken);

        var baseItems = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skipCount)
            .Take(take)
            .Select(x => new ListJobApplicationsItemDto
            {
                Id = x.Id,
                Title = x.Title,
                JobTitle = x.JobTitle,
                CompanyName = x.CompanyName,
                WorkType = (int)x.WorkType,
                CurrentStatus = (int)x.CurrentStatus,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Tags = new List<string>(),
            })
            .ToListAsync(cancellationToken);

        var jobApplicationIds = baseItems.Select(x => x.Id).ToList();

        var tagsByJobApplicationId = await dbContext.JobApplicationTags
            .Where(x => jobApplicationIds.Contains(x.JobApplicationId))
            .Join(
                dbContext.Tags,
                x => x.TagId,
                tag => tag.Id,
                (x, tag) => new { x.JobApplicationId, tag.Name })
            .GroupBy(x => x.JobApplicationId)
            .ToDictionaryAsync(
                x => x.Key,
                x => (IReadOnlyList<string>)x
                    .Select(y => y.Name)
                    .Distinct()
                    .OrderBy(y => y)
                    .ToList(),
                cancellationToken);

        var items = baseItems
            .Select(x => x with
            {
                Tags = tagsByJobApplicationId.GetValueOrDefault(x.Id, new List<string>()),
            })
            .ToList();

        return new ListJobApplicationsResponse
        {
            Total = total,
            Items = items,
        };
    }
}
