using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BeerSender.Projections.Database;

public class EventStoreConnectionFactory(IConfiguration configuration)
{
    private readonly string? _eventStoreConnectionString 
        = configuration.GetConnectionString("EventStore");    

    public IDbConnection CreateConnection() 
        => new SqlConnection(_eventStoreConnectionString);    
}