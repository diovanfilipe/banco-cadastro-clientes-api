using Microsoft.Data.Sqlite;

namespace CadastroClientes.Infrastructure.Persistence;

public sealed class SqliteConnectionFactory : ISqliteConnectionFactory
{
    private readonly string _connectionString;
    private readonly SqliteConnection _keepAliveConnection;
    private bool _disposed;

    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
        _keepAliveConnection = new SqliteConnection(connectionString);
        _keepAliveConnection.Open();
    }

    public SqliteConnection CreateConnection()
    {
        ThrowIfDisposed();

        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _keepAliveConnection.Dispose();
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(SqliteConnectionFactory));
        }
    }
}
