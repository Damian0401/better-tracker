using System.Security.Claims;
using BetterTracker.Common.Results;
using Microsoft.AspNetCore.Http;

namespace BetterTracker.Common.Helpers;

public static class UserIdHelper
{
    public static Result<Guid> GetUserId(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            return Result<Guid>.Failure("User is not authenticated");
        }

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Result<Guid>.Failure("Invalid user identifier");
        }

        return Result<Guid>.Success(userId);
    }
}
