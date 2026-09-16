using Baltec.configs;
using MySqlConnector;

namespace Baltec.DAO;

public abstract class BaseDAO
{
    protected readonly DatabaseConnection _db;

    protected BaseDAO(DatabaseConnection db)
    {
        _db = db;
    }

    protected async Task<MySqlConnection> OpenConnectionAsync()
    {
        var conn = _db.GetConnection();
        await conn.OpenAsync();
        return conn;
    }
}
