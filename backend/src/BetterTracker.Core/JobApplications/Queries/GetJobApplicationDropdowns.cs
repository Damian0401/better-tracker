using BetterTracker.Contracts;
using BetterTracker.Data.Entities;

namespace BetterTracker.Core.JobApplications.Queries;

public static class GetJobApplicationDropdowns
{
    public static ValueTask<GetJobApplicationDropdownsResponse> HandleAsync()
    {
        var response = new GetJobApplicationDropdownsResponse
        {
            WorkTypes = Enum.GetValues<WorkType>()
                .Select(x => new GetJobApplicationDropdownsResponse.EnumOption
                {
                    Value = (int)x,
                    Name = x.ToString(),
                })
                .ToList(),
            SalaryTypes = Enum.GetValues<SalaryType>()
                .Select(x => new GetJobApplicationDropdownsResponse.EnumOption
                {
                    Value = (int)x,
                    Name = x.ToString(),
                })
                .ToList(),
            JobApplicationStatuses = Enum.GetValues<JobApplicationStatus>()
                .Select(x => new GetJobApplicationDropdownsResponse.EnumOption
                {
                    Value = (int)x,
                    Name = x.ToString(),
                })
                .ToList(),
        };

        return ValueTask.FromResult(response);
    }
}
