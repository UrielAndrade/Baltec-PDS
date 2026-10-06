using MySqlConnector;

namespace Baltec.configs;

public class DatabaseConnection
{
    private readonly IConfiguration _configuration;

    public DatabaseConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(GetConnectionString());
    }

    public string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=localhost;Port=3360;Database=baltec;User=root;Password=root;Connection Timeout=15;";
    }
}
