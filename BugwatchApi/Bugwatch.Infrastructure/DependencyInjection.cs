using Bugwatch.Application;
using Bugwatch.Application.Constants;
using Bugwatch.Application.Interfaces;
using Bugwatch.Infrastructure.Context;
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

        services.AddSingleton<DapperContext>();

        services.AddSingleton<ITeamRepository, TeamRepository>()
            .AddSingleton<ITeamMemberRepository, TeamMemberRepository>()
            .AddProjectServices()
            .AddTicketServices();

        return services;
    }

    private static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddSingleton<IProjectRepository, ProjectRepository>()
            .AddSingleton<IRoleProjectRepository, DeveloperProjectRepository>()
            .AddSingleton<IRoleProjectRepository, ProjectManagerProjectRepository>();

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

    private static IServiceCollection AddTicketServices(this IServiceCollection services)
    {
        services.AddSingleton<ITicketRepository, TicketRepository>()
            .AddSingleton<IRoleTicketRepository,DeveloperTicketRepository>()
            .AddSingleton<IRoleTicketRepository, SubmitterTicketRepository>();

        services.AddSingleton<ITicketService, TicketService>();

        services.AddSingleton<ITicketHistoryRepository, TicketHistoryRepository>();
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