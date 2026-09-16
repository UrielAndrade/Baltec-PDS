using MySqlConnector;

namespace Baltec.configs;

public class DatabaseConnection
{
    private readonly string _connectionString;

    public DatabaseConnection(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentException("A string de conexão 'DefaultConnection' não foi encontrada.");
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }
}
