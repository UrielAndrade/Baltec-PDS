using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO;

public class UsuarioDAO : BaseDAO
{
    public UsuarioDAO(DatabaseConnection db) : base(db)
    {
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senhaPura)
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT u.*, c.Nome as CargoNome 
            FROM usuarios u 
            LEFT JOIN cargos c ON u.FkCargo = c.Id 
            WHERE u.Email = @email AND u.Ativo = 1";
        cmd.Parameters.AddWithValue("@email", email);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var hash = reader.GetString(reader.GetOrdinal("SenhaHash"));
            if (BCrypt.Net.BCrypt.Verify(senhaPura, hash))
            {
                return new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    NomeCompleto = reader.GetString(reader.GetOrdinal("NomeCompleto")),
                    Cpf = reader.GetString(reader.GetOrdinal("Cpf")),
                    Telefone = reader.IsDBNull(reader.GetOrdinal("Telefone")) ? null : reader.GetString(reader.GetOrdinal("Telefone")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    FkCargo = reader.GetInt32(reader.GetOrdinal("FkCargo")),
                    CargoNome = reader.IsDBNull(reader.GetOrdinal("CargoNome")) ? null : reader.GetString(reader.GetOrdinal("CargoNome")),
                    SenhaHash = hash,
                    DataCadastro = reader.GetDateTime(reader.GetOrdinal("DataCadastro")),
                    Ativo = reader.GetBoolean(reader.GetOrdinal("Ativo"))
                };
            }
        }
        return null;
    }

    public async Task<bool> CadastrarAsync(Usuario usuario, string senhaPura)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(senhaPura);
        using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO usuarios (NomeCompleto, Cpf, Telefone, Email, FkCargo, SenhaHash, DataCadastro, Ativo) 
            VALUES (@nome, @cpf, @telefone, @email, @fkCargo, @senhaHash, @dataCadastro, @ativo)";
        
        cmd.Parameters.AddWithValue("@nome", usuario.NomeCompleto);
        cmd.Parameters.AddWithValue("@cpf", usuario.Cpf);
        cmd.Parameters.AddWithValue("@telefone", string.IsNullOrEmpty(usuario.Telefone) ? DBNull.Value : usuario.Telefone);
        cmd.Parameters.AddWithValue("@email", usuario.Email);
        cmd.Parameters.AddWithValue("@fkCargo", usuario.FkCargo);
        cmd.Parameters.AddWithValue("@senhaHash", hash);
        cmd.Parameters.AddWithValue("@dataCadastro", DateTime.Now);
        cmd.Parameters.AddWithValue("@ativo", true);

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM usuarios WHERE Email = @email";
        cmd.Parameters.AddWithValue("@email", email);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }

    public async Task<bool> CpfExisteAsync(string cpf)
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM usuarios WHERE Cpf = @cpf";
        cmd.Parameters.AddWithValue("@cpf", cpf);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }

    public async Task<List<Cargo>> ListarCargosAsync()
    {
        var lista = new List<Cargo>();
        using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM cargos ORDER BY Nome";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Cargo
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nome = reader.GetString(reader.GetOrdinal("Nome")),
                Descricao = reader.IsDBNull(reader.GetOrdinal("Descricao")) ? null : reader.GetString(reader.GetOrdinal("Descricao"))
            });
        }
        return lista;
    }

    public async Task<List<Usuario>> ListarTecnicosAsync()
    {
        var lista = new List<Usuario>();
        using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT u.*, c.Nome as CargoNome 
            FROM usuarios u 
            JOIN cargos c ON u.FkCargo = c.Id 
            WHERE u.Ativo = 1 AND c.Nome LIKE '%Técnico%'";
        
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Usuario
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                NomeCompleto = reader.GetString(reader.GetOrdinal("NomeCompleto")),
                Cpf = reader.GetString(reader.GetOrdinal("Cpf")),
                Telefone = reader.IsDBNull(reader.GetOrdinal("Telefone")) ? null : reader.GetString(reader.GetOrdinal("Telefone")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                FkCargo = reader.GetInt32(reader.GetOrdinal("FkCargo")),
                CargoNome = reader.IsDBNull(reader.GetOrdinal("CargoNome")) ? null : reader.GetString(reader.GetOrdinal("CargoNome")),
                SenhaHash = reader.GetString(reader.GetOrdinal("SenhaHash")),
                DataCadastro = reader.GetDateTime(reader.GetOrdinal("DataCadastro")),
                Ativo = reader.GetBoolean(reader.GetOrdinal("Ativo"))
            });
        }
        return lista;
    }
}
