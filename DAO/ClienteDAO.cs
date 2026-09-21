using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO
{
    public class ClienteDAO
    {
        private readonly string connectionString =
            "Server=localhost;Database=baltec;Uid=root;Pwd=;";

        public async Task<List<Cliente>> ListarTodosAsync()
        {
            var clientes = new List<Cliente>();

            using var conexao = new MySqlConnection(connectionString);
            await conexao.OpenAsync();

            string sql = @"SELECT 
                            id,
                            razao_social,
                            nome_fantasia,
                            cnpj_cpf,
                            telefone,
                            email,
                            endereco,
                            cidade,
                            estado,
                            data_cadastro,
                            ativo
                           FROM cliente";

            using var comando = new MySqlCommand(sql, conexao);
            using var reader = await comando.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                clientes.Add(new Cliente
                {
                    Id = reader.GetInt32("id"),
                    RazaoSocial = reader.GetString("razao_social"),
                    NomeFantasia = reader.IsDBNull(reader.GetOrdinal("nome_fantasia"))
                        ? null
                        : reader.GetString("nome_fantasia"),
                    CnpjCpf = reader.GetString("cnpj_cpf"),
                    Telefone = reader.IsDBNull(reader.GetOrdinal("telefone"))
                        ? null
                        : reader.GetString("telefone"),
                    Email = reader.IsDBNull(reader.GetOrdinal("email"))
                        ? null
                        : reader.GetString("email"),
                    Endereco = reader.IsDBNull(reader.GetOrdinal("endereco"))
                        ? null
                        : reader.GetString("endereco"),
                    Cidade = reader.IsDBNull(reader.GetOrdinal("cidade"))
                        ? null
                        : reader.GetString("cidade"),
                    Estado = reader.IsDBNull(reader.GetOrdinal("estado"))
                        ? null
                        : reader.GetString("estado"),
                    DataCadastro = reader.GetDateTime("data_cadastro"),
                    Ativo = reader.GetBoolean("ativo")
                });
            }

            return clientes;
        }

        public async Task<Cliente?> BuscarPorIdAsync(int id)
        {
            using var conexao = new MySqlConnection(connectionString);
            await conexao.OpenAsync();

            string sql = @"SELECT 
                            id,
                            razao_social,
                            nome_fantasia,
                            cnpj_cpf,
                            telefone,
                            email,
                            endereco,
                            cidade,
                            estado,
                            data_cadastro,
                            ativo
                           FROM cliente
                           WHERE id = @id";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.AddWithValue("@id", id);

            using var reader = await comando.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Cliente
                {
                    Id = reader.GetInt32("id"),
                    RazaoSocial = reader.GetString("razao_social"),
                    NomeFantasia = reader.IsDBNull(reader.GetOrdinal("nome_fantasia"))
                        ? null
                        : reader.GetString("nome_fantasia"),
                    CnpjCpf = reader.GetString("cnpj_cpf"),
                    Telefone = reader.IsDBNull(reader.GetOrdinal("telefone"))
                        ? null
                        : reader.GetString("telefone"),
                    Email = reader.IsDBNull(reader.GetOrdinal("email"))
                        ? null
                        : reader.GetString("email"),
                    Endereco = reader.IsDBNull(reader.GetOrdinal("endereco"))
                        ? null
                        : reader.GetString("endereco"),
                    Cidade = reader.IsDBNull(reader.GetOrdinal("cidade"))
                        ? null
                        : reader.GetString("cidade"),
                    Estado = reader.IsDBNull(reader.GetOrdinal("estado"))
                        ? null
                        : reader.GetString("estado"),
                    DataCadastro = reader.GetDateTime("data_cadastro"),
                    Ativo = reader.GetBoolean("ativo")
                };
            }

            return null;
        }

        public async Task<bool> InserirAsync(Cliente cliente)
        {
            using var conexao = new MySqlConnection(connectionString);
            await conexao.OpenAsync();

            string sql = @"INSERT INTO cliente
                           (razao_social, nome_fantasia, cnpj_cpf, telefone,
                            email, endereco, cidade, estado, ativo)
                           VALUES
                           (@razao_social, @nome_fantasia, @cnpj_cpf, @telefone,
                            @email, @endereco, @cidade, @estado, @ativo)";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@razao_social", cliente.RazaoSocial);
            comando.Parameters.AddWithValue("@nome_fantasia", cliente.NomeFantasia);
            comando.Parameters.AddWithValue("@cnpj_cpf", cliente.CnpjCpf);
            comando.Parameters.AddWithValue("@telefone", cliente.Telefone);
            comando.Parameters.AddWithValue("@email", cliente.Email);
            comando.Parameters.AddWithValue("@endereco", cliente.Endereco);
            comando.Parameters.AddWithValue("@cidade", cliente.Cidade);
            comando.Parameters.AddWithValue("@estado", cliente.Estado);
            comando.Parameters.AddWithValue("@ativo", cliente.Ativo);

            return await comando.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> AtualizarAsync(Cliente cliente)
        {
            using var conexao = new MySqlConnection(connectionString);
            await conexao.OpenAsync();

            string sql = @"UPDATE cliente SET
                            razao_social = @razao_social,
                            nome_fantasia = @nome_fantasia,
                            cnpj_cpf = @cnpj_cpf,
                            telefone = @telefone,
                            email = @email,
                            endereco = @endereco,
                            cidade = @cidade,
                            estado = @estado,
                            ativo = @ativo
                           WHERE id = @id";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@id", cliente.Id);
            comando.Parameters.AddWithValue("@razao_social", cliente.RazaoSocial);
            comando.Parameters.AddWithValue("@nome_fantasia", cliente.NomeFantasia);
            comando.Parameters.AddWithValue("@cnpj_cpf", cliente.CnpjCpf);
            comando.Parameters.AddWithValue("@telefone", cliente.Telefone);
            comando.Parameters.AddWithValue("@email", cliente.Email);
            comando.Parameters.AddWithValue("@endereco", cliente.Endereco);
            comando.Parameters.AddWithValue("@cidade", cliente.Cidade);
            comando.Parameters.AddWithValue("@estado", cliente.Estado);
            comando.Parameters.AddWithValue("@ativo", cliente.Ativo);

            return await comando.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DesativarAsync(int id)
        {
            using var conexao = new MySqlConnection(connectionString);
            await conexao.OpenAsync();

            string sql = @"UPDATE cliente
                           SET ativo = FALSE
                           WHERE id = @id";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.AddWithValue("@id", id);

            return await comando.ExecuteNonQueryAsync() > 0;
        }
    }
}