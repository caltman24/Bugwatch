using Bugwatch.Application.Entities;
using Bugwatch.Infrastructure.Context;
using Dapper;
using Npgsql;

namespace Bugwatch.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _dapperContext;

    public UserRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task Add(User newUser)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"
            INSERT INTO ""user"" (id, auth_id, created_at, updated_at, email, name)
            VALUES (@Id, @AuthId, @CreatedAt, @UpdatedAt, @Email, @Name);";

        await conn.ExecuteAsync(sql, new
        {
            newUser.Id,
            newUser.AuthId,
            newUser.CreatedAt,
            newUser.UpdatedAt,
            newUser.Email,
            newUser.Name
        });
    }

    public async Task<bool> UserExists(string authId)
    {
        using var conn = _dapperContext.CreateConnection();

        const string sql = @"SELECT EXISTS (SELECT 1 FROM ""user"" u WHERE u.auth_id = @authId);";

        return await conn.ExecuteScalarAsync<bool>(sql, new { authId });
    }
}

public interface IUserRepository
{
    Task Add(User newUser);
    Task<bool> UserExists(string authId);
}