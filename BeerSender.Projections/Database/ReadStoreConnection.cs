using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BeerSender.Projections.Database;

public class ReadStoreConnection(IConfiguration configuration) : IDisposable
{
    private readonly string? _readStoreConnectionString
        = configuration.GetConnectionString("ReadStore");

    private SqlConnection? _connection;
    private SqlTransaction? _transaction;

    public IDbConnection GetConnection()
    {
        if(_connection is null)
            _connection = new SqlConnection(_readStoreConnectionString);

        return _connection;
    }

    public IDbTransaction GetTransaction()
    {
        if (_transaction is not null) 
            return _transaction;
        
        if (_connection is null)
            _connection = new SqlConnection(_readStoreConnectionString);

        if (_connection.State == ConnectionState.Closed)
            _connection.Open();

        _transaction = _connection.BeginTransaction();
        return _transaction;
    }

    public void Dispose()
    {
        if(_transaction is not null)
            _transaction.Dispose();
        if(_connection is not null)
            _connection.Dispose();
    }
}   