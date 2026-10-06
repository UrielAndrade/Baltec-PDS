using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO;

public class FornecedorDAO : BaseDAO
{
    public FornecedorDAO(DatabaseConnection db) : base(db)
    {
    }

    public async Task<List<Fornecedor>> ListarTodosAsync()
    {
        var lista = new List<Fornecedor>();
        using var conexao = await OpenConnectionAsync();

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
                       FROM fornecedor
                       ORDER BY razao_social";

        using var comando = new MySqlCommand(sql, conexao);
        using var reader = await comando.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Fornecedor
            {
                Id = reader.GetInt32("id"),
                RazaoSocial = reader.GetString("razao_social"),
                NomeFantasia = reader.IsDBNull(reader.GetOrdinal("nome_fantasia")) ? null : reader.GetString("nome_fantasia"),
                CnpjCpf = reader.GetString("cnpj_cpf"),
                Telefone = reader.IsDBNull(reader.GetOrdinal("telefone")) ? null : reader.GetString("telefone"),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
                Endereco = reader.IsDBNull(reader.GetOrdinal("endereco")) ? null : reader.GetString("endereco"),
                Cidade = reader.IsDBNull(reader.GetOrdinal("cidade")) ? null : reader.GetString("cidade"),
                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString("estado"),
                DataCadastro = reader.GetDateTime("data_cadastro"),
                Ativo = reader.GetBoolean("ativo")
            });
        }

        return lista;
    }
}
