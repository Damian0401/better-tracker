using BetterTracker.Contracts;
using BetterTracker.Data;
using BetterTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Core.JobApplications.Queries;

public static class GetJobApplicationStatistics
{
    public static async ValueTask<GetJobApplicationStatisticsResponse> HandleAsync(
        DateTimeOffset? dateFrom,
        DateTimeOffset? dateTo,
        bool? includeArchived,
        Guid userId,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var query = dbContext.JobApplications
            .Where(x => x.UserId == userId)
            .AsQueryable();

        if (includeArchived is not true)
        {
            query = query.Where(x => !x.IsArchived);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= dateTo.Value);
        }

        var groupedStatusCounts = await query
            .GroupBy(x => x.CurrentStatus)
            .Select(x => new
            {
                Status = x.Key,
                Count = x.Count(),
            })
            .ToDictionaryAsync(
                x => x.Status,
                x => x.Count,
                cancellationToken);

        var statusCounts = Enum.GetValues<JobApplicationStatus>()
            .Select(x => new GetJobApplicationStatisticsStatusCountDto
            {
                Status = (int)x,
                Count = groupedStatusCounts.GetValueOrDefault(x, 0),
            })
            .ToList();

        return new GetJobApplicationStatisticsResponse
        {
            Total = statusCounts.Sum(x => x.Count),
            StatusCounts = statusCounts,
        };
    }
}
