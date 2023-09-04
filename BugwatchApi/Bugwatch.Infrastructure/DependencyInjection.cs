using Bugwatch.Application;
using Bugwatch.Application.Constants;
using Bugwatch.Application.Interfaces;
using Bugwatch.Infrastructure.Factories;
using Bugwatch.Infrastructure.Repositories;
using Bugwatch.Infrastructure.Repositories.Projects;
using Bugwatch.Infrastructure.Repositories.Tickets;
using Bugwatch.Infrastructure.Services;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bugwatch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        var connectionString = config.GetConnectionString("Cosmos")!;

        services.AddSingleton<ITeamRepository, TeamRepository>(_ =>
                new TeamRepository(connectionString))
            .AddSingleton<ITeamMemberRepository, TeamMemberRepository>(_ =>
                new TeamMemberRepository(connectionString))
            .AddProjectServices(connectionString)
            .AddTicketServices(connectionString);

        return services;
    }

    private static IServiceCollection AddProjectServices(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IProjectRepository, ProjectRepository>(_ => new ProjectRepository(connectionString))
            .AddSingleton<DeveloperProjectRepository>(_ => new DeveloperProjectRepository(connectionString))
            .AddSingleton<ProjectManagerProjectRepository>(_ => new ProjectManagerProjectRepository(connectionString));

        services.AddSingleton<ProjectRepositoryResolver>(sp => role =>
        {
            return (role switch
            {
                UserRoles.Developer => sp.GetService<DeveloperProjectRepository>(),
                UserRoles.ProjectManager => sp.GetService<ProjectManagerProjectRepository>(),
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            })!;
        });

        return services;
    }

    private static IServiceCollection AddTicketServices(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<ITicketRepository, TicketRepository>(_ => new TicketRepository(connectionString))
            .AddSingleton<DeveloperTicketRepository>(_ => new DeveloperTicketRepository(connectionString))
            .AddSingleton<SubmitterTicketRepository>(_ => new SubmitterTicketRepository(connectionString));

        services.AddSingleton<ITicketService, TicketService>();

        services.AddSingleton<ITicketHistoryRepository, TicketHistoryRepository>(_ =>
            new TicketHistoryRepository(connectionString));
        services.AddSingleton<ITicketHistoryService, TicketHistoryService>();
        services.AddSingleton<ITicketHistoryFactory, TicketHistoryFactory>();

        services.AddSingleton<TicketRepositoryResolver>(sp => role =>
        {
            return (role switch
            {
                UserRoles.Developer => sp.GetService<DeveloperTicketRepository>(),
                UserRoles.Submitter => sp.GetService<SubmitterTicketRepository>(),
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            })!;
        });

        return services;
    }
}