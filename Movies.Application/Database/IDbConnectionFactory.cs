using Npgsql;
using System.Data;

namespace Movies.Application.Database
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    }

    public class NgsqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public NgsqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            return connection;
        }
    }
}
