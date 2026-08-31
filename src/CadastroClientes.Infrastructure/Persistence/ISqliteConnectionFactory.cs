using Microsoft.Data.Sqlite;

namespace CadastroClientes.Infrastructure.Persistence;

public interface ISqliteConnectionFactory : IDisposable
{
    SqliteConnection CreateConnection();
}
