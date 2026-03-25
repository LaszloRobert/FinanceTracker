using System.Text;
using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.BankData;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Infrastructure.Authentication;
using FinanceTracker.Infrastructure.BankData;
using FinanceTracker.Infrastructure.Database;
using FinanceTracker.Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FinanceTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddJwtAuthentication(configuration);
        services.AddBankData(configuration);
        services.AddServices();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")!;

        services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        JwtOptions jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });

        services.AddAuthorization();

        services.AddHttpContextAccessor();

        return services;
    }

    private static IServiceCollection AddBankData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<GoCardlessOptions>()
            .BindConfiguration(GoCardlessOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        GoCardlessOptions goCardlessOptions = configuration
            .GetSection(GoCardlessOptions.SectionName)
            .Get<GoCardlessOptions>()!;

        services.AddSingleton<GoCardlessTokenCache>();

        services.AddHttpClient<GoCardlessTokenService>(client =>
        {
            client.BaseAddress = new Uri(goCardlessOptions.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddHttpClient<IBankDataService, GoCardlessBankDataService>(client =>
        {
            client.BaseAddress = new Uri(goCardlessOptions.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}
