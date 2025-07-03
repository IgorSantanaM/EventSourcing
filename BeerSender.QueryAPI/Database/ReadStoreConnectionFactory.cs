using System.Data;
using Microsoft.Data.SqlClient;

namespace BeerSender.QueryAPI.Database;

public class ReadStoreConnectionFactory(IConfiguration configuration)
{
    private readonly string? _connectionString 
        = configuration.GetConnectionString("ReadStore");

    public IDbConnection Create() 
        => new SqlConnection(_connectionString);
}