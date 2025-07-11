﻿using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace BeerSender.Domain.Tests
{
    public class MartenFixture : IDisposable
    {
        private readonly string schema = $"bstest{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
        private readonly IHost? _host;
        public IDocumentStore Store { get; private set; }

        public MartenFixture()
        {
            CreateSchema();
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddMarten(options =>
                    {
                        options.Connection(GetConnectionString());
                        options.DatabaseSchemaName = schema;

                        options.ApplyDomainConfig();
                        options.AddProjections();
                    }).AddAsyncDaemon(JasperFx.Events.Daemon.DaemonMode.Solo);
                }).Start();

            Store = _host.Services.GetRequiredService<IDocumentStore>();
        }

        public void Dispose()
        {
            if(_host != null) _host.Dispose();
            DropSchema(schema);
        }

        private void CreateSchema()
        {
            using var connection = new NpgsqlConnection(GetConnectionString());
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"CREATE SCHEMA IF NOT EXISTS {schema}";
            command.ExecuteNonQuery();
            connection.Close();
        }

        private void DropSchema(string s)
        {
            using var connection = new NpgsqlConnection(GetConnectionString());
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"DROP SCHEMA IF EXISTS {schema} CASCADE";
            command.ExecuteNonQuery();
            connection.Close();
        }

        private string? _connectionString;
        private string GetConnectionString()
        {
            if (_connectionString == null)
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                _connectionString = config.GetConnectionString("Marten");

                if (_connectionString is null)
                    throw new Exception("ConnectionString unavailable.");
            }
            return _connectionString;
        }
    }
    [CollectionDefinition("Marten collection")]
    public class DatabaseCollection : ICollectionFixture<MartenFixture>
    {

    }
}
