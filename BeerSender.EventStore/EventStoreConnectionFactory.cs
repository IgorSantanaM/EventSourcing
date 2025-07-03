using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BeerSender.EventStore;

public class EventStoreConnectionFactory(IConfiguration configuration)
{
    private readonly string? _connectionString 
        = configuration.GetConnectionString("EventStore");

    public IDbConnection Create() 
        => new SqlConnection(_connectionString);
}