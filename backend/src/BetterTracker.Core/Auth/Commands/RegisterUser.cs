using BetterTracker.Common.Results;
using BetterTracker.Contracts;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.Auth.Commands;

public static class RegisterUser
{
    public static async Task<Result<UserEntity>> HandleAsync(
        RegisterRequest request,
        IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByLoginAsync(request.Login, cancellationToken);
        if (existingUser is not null)
        {
            return Result<UserEntity>.Failure("User with this login already exists");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new UserEntity
        {
            UserName = request.UserName,
            Login = request.Login,
            PasswordHash = passwordHash,
        };

        userRepository.Add(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result<UserEntity>.Success(user);
    }
}
