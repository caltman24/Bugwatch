using Bugwatch.Application.Entities;

namespace Bugwatch.Application.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByAuthIdAsync(string authId);
    Task<Team?> GetByUserIdAsync(Guid userId);
    Task<bool> InsertAsync(Team team, string authId);
    Task UpdateAsync(Guid teamId, string name);
    Task DeleteAsync(Guid teamId);
}