using Dapper;

namespace CadastroClientes.Infrastructure.Persistence;

public sealed class SqliteSchemaInitializer
{
    private readonly ISqliteConnectionFactory _connectionFactory;

    public SqliteSchemaInitializer(ISqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Initialize()
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS clients (
                id TEXT NOT NULL PRIMARY KEY,
                name TEXT NOT NULL,
                cpf TEXT NOT NULL UNIQUE,
                email TEXT NOT NULL UNIQUE,
                created_at TEXT NOT NULL
            );
            """;

        using var connection = _connectionFactory.CreateConnection();
        connection.Execute(sql);
    }
}
