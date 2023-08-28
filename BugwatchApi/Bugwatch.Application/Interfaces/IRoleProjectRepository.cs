using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface IRoleProjectRepository
{
    Task<IQueryable<BasicProject>> GetProjectsAsync(string authId);
}