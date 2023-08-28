using Bugwatch.Application.Aggregates;
using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface IProjectRepository
{
    Task<IQueryable<BasicProject>> GetAllAsync(string authId);
    Task<Project?> GetByIdAsync(Guid projectId);
    Task InsertAsync(BasicProject project);
    Task UpdateAsync(Guid projectId, BasicProject updated);
    Task DeleteAsync(Guid projectId);
}