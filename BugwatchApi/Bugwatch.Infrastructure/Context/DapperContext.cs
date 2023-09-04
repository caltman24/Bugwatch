using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Bugwatch.Infrastructure.Context;

public sealed class DapperContext
{
    private readonly IConfiguration _configuration;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_configuration.GetConnectionString("Development"));
}