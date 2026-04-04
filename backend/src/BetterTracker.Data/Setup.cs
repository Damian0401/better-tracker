using BetterTracker.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BetterTracker.Data;

public static class Setup
{
    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=bettertracker.db";

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString, 
                b => b.MigrationsAssembly("BetterTracker.Data.Migrations")));

        return builder;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<INoteRepository, NoteRepository>();
        return services;
    }
}
