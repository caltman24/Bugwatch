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

        services.AddTransient<ITeamRepository, TeamRepository>()
            .AddTransient<ITeamMemberRepository, TeamMemberRepository>()
            .AddProjectServices()
            .AddTicketServices();

        return services;
    }

    private static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddTransient<IProjectRepository, ProjectRepository>()
            .AddTransient<IRoleProjectRepository, DeveloperProjectRepository>()
            .AddTransient<IRoleProjectRepository, ProjectManagerProjectRepository>();

        services.AddTransient<ProjectRepositoryResolver>(sp => role =>
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
        services.AddTransient<ITicketRepository, TicketRepository>()
            .AddTransient<IRoleTicketRepository,DeveloperTicketRepository>()
            .AddTransient<IRoleTicketRepository, SubmitterTicketRepository>();

        services.AddTransient<ITicketService, TicketService>();

        services.AddTransient<ITicketHistoryRepository, TicketHistoryRepository>();
        services.AddTransient<ITicketHistoryService, TicketHistoryService>();
        services.AddTransient<ITicketHistoryFactory, TicketHistoryFactory>();

        services.AddTransient<TicketRepositoryResolver>(sp => role =>
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