using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByAuthIdAsync(string authId);
    Task<Team?> GetByUserIdAsync(Guid userId);
    Task InsertAsync(Team team);
    Task UpdateAsync(Guid teamId, string name);
    Task DeleteAsync(Guid teamId);
}