using BetterTracker.Contracts;
using BetterTracker.Data;
using BetterTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Core.JobApplications.Queries;

public static class ListJobApplications
{
    private const int DefaultCount = 10;
    private const string ActiveState = "active";
    private const string ArchivedState = "archived";
    private const string AllState = "all";

    public static async ValueTask<ListJobApplicationsResponse> HandleAsync(
        int? count,
        int? skip,
        IReadOnlyList<int>? statuses,
        IReadOnlyList<string>? tags,
        IReadOnlyList<int>? workTypes,
        string? state,
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

        var normalizedState = NormalizeState(state);
        query = normalizedState switch
        {
            ArchivedState => query.Where(x => x.IsArchived),
            AllState => query,
            _ => query.Where(x => !x.IsArchived),
        };

        var parsedStatuses = statuses?
            .Where(x => Enum.IsDefined((JobApplicationStatus)x))
            .Select(x => (JobApplicationStatus)x)
            .Distinct()
            .ToList();

        if (parsedStatuses is { Count: > 0 })
        {
            query = query.Where(x => parsedStatuses.Contains(x.CurrentStatus));
        }

        var parsedWorkTypes = workTypes?
            .Where(x => Enum.IsDefined((WorkType)x))
            .Select(x => (WorkType)x)
            .Distinct()
            .ToList();

        if (parsedWorkTypes is { Count: > 0 })
        {
            query = query.Where(x => parsedWorkTypes.Contains(x.WorkType));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();
            query = query.Where(x =>
                x.JobTitle.Contains(normalizedSearch) ||
                x.CompanyName.Contains(normalizedSearch));
        }

        var normalizedTags = tags?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedTags is { Count: > 0 })
        {
            query = query.Where(x => dbContext.JobApplicationTags
                .Where(y => y.JobApplicationId == x.Id)
                .Join(
                    dbContext.Tags,
                    y => y.TagId,
                    t => t.Id,
                    (_, t) => t.Name)
                .Any(t => normalizedTags.Contains(t)));
        }

        var total = await query.CountAsync(cancellationToken);

        var baseItems = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skipCount)
            .Take(take)
            .Select(x => new ListJobApplicationsItemDto
            {
                Id = x.Id,
                JobTitle = x.JobTitle,
                CompanyName = x.CompanyName,
                WorkType = (int)x.WorkType,
                CurrentStatus = (int)x.CurrentStatus,
                IsArchived = x.IsArchived,
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

    private static string NormalizeState(string? state)
    {
        if (string.IsNullOrWhiteSpace(state))
        {
            return ActiveState;
        }

        var normalizedState = state.Trim().ToLowerInvariant();
        return normalizedState is ArchivedState or AllState
            ? normalizedState
            : ActiveState;
    }
}
