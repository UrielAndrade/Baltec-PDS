using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO;

public class UsuarioDAO : BaseDAO
{
    public UsuarioDAO(DatabaseConnection db) : base(db)
    {
    }

    private async Task EnsureTablesCreatedAsync(MySqlConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS cargo (
                id INT AUTO_INCREMENT PRIMARY KEY,
                nome VARCHAR(100) NOT NULL,
                descricao VARCHAR(255) NULL
            );

            CREATE TABLE IF NOT EXISTS usuario (
                id INT AUTO_INCREMENT PRIMARY KEY,
                nome_completo VARCHAR(150) NOT NULL,
                cpf VARCHAR(14) NOT NULL UNIQUE,
                telefone VARCHAR(15) NULL,
                email VARCHAR(100) NOT NULL UNIQUE,
                fk_cargo INT NOT NULL,
                senha_hash VARCHAR(255) NOT NULL,
                data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
                ativo BOOLEAN DEFAULT TRUE
            );

            INSERT IGNORE INTO cargo (id, nome, descricao) VALUES 
            (1, 'Técnico de Calibração', 'Técnico responsável pelas aferições'),
            (2, 'Engenheiro', 'Engenheiro metrologista'),
            (3, 'Administrativo', 'Equipe de escritório e atendimento'),
            (4, 'Gerente', 'Gerente da unidade');
        ";
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senhaPura)
    {
        using var conn = await OpenConnectionAsync();
        await EnsureTablesCreatedAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT u.id, u.nome_completo, u.cpf, u.telefone, u.email, u.fk_cargo, u.senha_hash, u.data_cadastro, u.ativo,
                   c.nome AS CargoNome 
            FROM usuario u 
            LEFT JOIN cargo c ON u.fk_cargo = c.id 
            WHERE u.email = @email AND u.ativo = 1
            LIMIT 1";
        cmd.Parameters.AddWithValue("@email", email);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var hash = reader.GetString(reader.GetOrdinal("senha_hash"));
            if (BCrypt.Net.BCrypt.Verify(senhaPura, hash))
            {
                return new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    NomeCompleto = reader.GetString(reader.GetOrdinal("nome_completo")),
                    Cpf = reader.GetString(reader.GetOrdinal("cpf")),
                    Telefone = reader.IsDBNull(reader.GetOrdinal("telefone")) ? null : reader.GetString(reader.GetOrdinal("telefone")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    FkCargo = reader.GetInt32(reader.GetOrdinal("fk_cargo")),
                    CargoNome = reader.IsDBNull(reader.GetOrdinal("CargoNome")) ? null : reader.GetString(reader.GetOrdinal("CargoNome")),
                    SenhaHash = hash,
                    DataCadastro = reader.GetDateTime(reader.GetOrdinal("data_cadastro")),
                    Ativo = reader.GetBoolean(reader.GetOrdinal("ativo"))
                };
            }
        }
        return null;
    }

    public async Task<bool> CadastrarAsync(Usuario usuario, string senhaPura)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(senhaPura);
        using var conn = await OpenConnectionAsync();
        await EnsureTablesCreatedAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO usuario (nome_completo, cpf, telefone, email, fk_cargo, senha_hash, data_cadastro, ativo) 
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
        await EnsureTablesCreatedAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM usuario WHERE email = @email";
        cmd.Parameters.AddWithValue("@email", email);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }

    public async Task<bool> CpfExisteAsync(string cpf)
    {
        using var conn = await OpenConnectionAsync();
        await EnsureTablesCreatedAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM usuario WHERE cpf = @cpf";
        cmd.Parameters.AddWithValue("@cpf", cpf);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }

    public async Task<List<Cargo>> ListarCargosAsync()
    {
        var lista = new List<Cargo>();
        using var conn = await OpenConnectionAsync();
        await EnsureTablesCreatedAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nome, descricao FROM cargo ORDER BY nome";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Cargo
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? null : reader.GetString(reader.GetOrdinal("descricao"))
            });
        }
        return lista;
    }

    public async Task<List<Usuario>> ListarTecnicosAsync()
    {
        var lista = new List<Usuario>();
        using var conn = await OpenConnectionAsync();
        await EnsureTablesCreatedAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT u.id, u.nome_completo, u.cpf, u.telefone, u.email, u.fk_cargo, u.senha_hash, u.data_cadastro, u.ativo,
                   c.nome AS CargoNome 
            FROM usuario u 
            JOIN cargo c ON u.fk_cargo = c.id 
            WHERE u.ativo = 1 AND c.nome LIKE '%Técnico%'";
        
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Usuario
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                NomeCompleto = reader.GetString(reader.GetOrdinal("nome_completo")),
                Cpf = reader.GetString(reader.GetOrdinal("cpf")),
                Telefone = reader.IsDBNull(reader.GetOrdinal("telefone")) ? null : reader.GetString(reader.GetOrdinal("telefone")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                FkCargo = reader.GetInt32(reader.GetOrdinal("fk_cargo")),
                CargoNome = reader.IsDBNull(reader.GetOrdinal("CargoNome")) ? null : reader.GetString(reader.GetOrdinal("CargoNome")),
                SenhaHash = reader.GetString(reader.GetOrdinal("senha_hash")),
                DataCadastro = reader.GetDateTime(reader.GetOrdinal("data_cadastro")),
                Ativo = reader.GetBoolean(reader.GetOrdinal("ativo"))
            });
        }
        return lista;
    }
}
