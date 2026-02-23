using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syndicate.Domain.Interfaces;
using Syndicate.Infrastructure.Auth;
using Syndicate.Infrastructure.Data;
using Syndicate.Infrastructure.Repositories;

namespace Syndicate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SyndicateDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SyndicateDbContext>());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IDebtRepository, DebtRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IPollRepository, PollRepository>();

        // Auth services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
