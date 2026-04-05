using BetterTracker.Common.Results;
using BetterTracker.Contracts;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.Auth.Commands;

public static class LoginUser
{
    public static async Task<Result<UserEntity>> HandleAsync(
        LoginRequest request,
        IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByLoginAsync(request.Login, cancellationToken);
        if (user is null)
        {
            return Result<UserEntity>.Failure("Invalid login or password");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<UserEntity>.Failure("Invalid login or password");
        }

        return Result<UserEntity>.Success(user);
    }
}
