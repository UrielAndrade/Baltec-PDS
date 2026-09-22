using Baltec.models;

using MySqlConnector;

namespace Baltec.DAO
{
    public class ComponenteDAO
    {
        private readonly string connectionString;

        public ComponenteDAO(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("String de conexão não encontrada.");
        }

        // Lista todos os componentes cadastrados
        public async Task<List<Componente>> ListarTodosAsync()
        {
            List<Componente> componentes = new List<Componente>();

            using var conexao = new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            string sql = @"
                SELECT 
                    c.id,
                    c.nome,
                    c.codigo_item,
                    c.fk_categoria,
                    cat.nome AS categoria_nome,
                    c.quantidade_estoque,
                    c.preco_unitario,
                    c.data_cadastro,
                    c.ativo
                FROM componente c
                INNER JOIN categoria_componente cat
                    ON cat.id = c.fk_categoria
                WHERE c.ativo = TRUE
                ORDER BY c.nome;
            ";

            using var comando = new MySqlCommand(sql, conexao);

            using var reader = await comando.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                componentes.Add(new Componente
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nome = reader["nome"].ToString() ?? "",
                    CodigoItem = reader["codigo_item"].ToString() ?? "",
                    FkCategoria = Convert.ToInt32(reader["fk_categoria"]),
                    CategoriaNome = reader["categoria_nome"].ToString(),
                    QuantidadeEstoque = Convert.ToInt32(reader["quantidade_estoque"]),
                    PrecoUnitario = Convert.ToDecimal(reader["preco_unitario"]),
                    DataCadastro = Convert.ToDateTime(reader["data_cadastro"]),
                    Ativo = Convert.ToBoolean(reader["ativo"])
                });
            }

            return componentes;
        }

        // Lista as categorias
        public async Task<List<CategoriaComponente>> ListarCategoriasAsync()
        {
            List<CategoriaComponente> categorias = new List<CategoriaComponente>();

            using var conexao = new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            string sql = @"
                SELECT id, nome
                FROM categoria_componente
                ORDER BY nome;
            ";

            using var comando = new MySqlCommand(sql, conexao);

            using var reader = await comando.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categorias.Add(new CategoriaComponente
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nome = reader["nome"].ToString() ?? ""
                });
            }

            return categorias;
        }

        // Procura um componente pelo código
        public async Task<Componente?> BuscarPorCodigoAsync(string codigoItem)
        {
            using var conexao = new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            string sql = @"
                SELECT 
                    c.id,
                    c.nome,
                    c.codigo_item,
                    c.fk_categoria,
                    cat.nome AS categoria_nome,
                    c.quantidade_estoque,
                    c.preco_unitario,
                    c.data_cadastro,
                    c.ativo
                FROM componente c
                INNER JOIN categoria_componente cat
                    ON cat.id = c.fk_categoria
                WHERE c.codigo_item = @codigo;
            ";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@codigo", codigoItem);

            using var reader = await comando.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Componente
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nome = reader["nome"].ToString() ?? "",
                    CodigoItem = reader["codigo_item"].ToString() ?? "",
                    FkCategoria = Convert.ToInt32(reader["fk_categoria"]),
                    CategoriaNome = reader["categoria_nome"].ToString(),
                    QuantidadeEstoque = Convert.ToInt32(reader["quantidade_estoque"]),
                    PrecoUnitario = Convert.ToDecimal(reader["preco_unitario"]),
                    DataCadastro = Convert.ToDateTime(reader["data_cadastro"]),
                    Ativo = Convert.ToBoolean(reader["ativo"])
                };
            }

            return null;
        }

        // Cadastra um novo componente
        public async Task<bool> InserirAsync(Componente componente)
        {
            using var conexao = new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            string sql = @"
                INSERT INTO componente
                (
                    nome,
                    codigo_item,
                    fk_categoria,
                    quantidade_estoque,
                    preco_unitario,
                    ativo
                )
                VALUES
                (
                    @nome,
                    @codigo,
                    @categoria,
                    @quantidade,
                    @preco,
                    TRUE
                );
            ";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@nome", componente.Nome);
            comando.Parameters.AddWithValue("@codigo", componente.CodigoItem);
            comando.Parameters.AddWithValue("@categoria", componente.FkCategoria);
            comando.Parameters.AddWithValue("@quantidade", componente.QuantidadeEstoque);
            comando.Parameters.AddWithValue("@preco", componente.PrecoUnitario);

            int resultado = await comando.ExecuteNonQueryAsync();

            return resultado > 0;
        }

        // Atualiza os dados de um componente
        public async Task<bool> AtualizarAsync(Componente componente)
        {
            using var conexao = new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            string sql = @"
                UPDATE componente
                SET
                    nome = @nome,
                    codigo_item = @codigo,
                    fk_categoria = @categoria,
                    preco_unitario = @preco
                WHERE id = @id;
            ";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@nome", componente.Nome);
            comando.Parameters.AddWithValue("@codigo", componente.CodigoItem);
            comando.Parameters.AddWithValue("@categoria", componente.FkCategoria);
            comando.Parameters.AddWithValue("@preco", componente.PrecoUnitario);
            comando.Parameters.AddWithValue("@id", componente.Id);

            int resultado = await comando.ExecuteNonQueryAsync();

            return resultado > 0;
        }

        // Aumenta ou diminui a quantidade do estoque
        public async Task<bool> AtualizarQuantidadeEstoqueAsync(
            int idComponente,
            int deltaQuantidade)
        {
            using var conexao = new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            string sql = @"
                UPDATE componente
                SET quantidade_estoque = quantidade_estoque + @delta
                WHERE id = @id
                  AND quantidade_estoque + @delta >= 0;
            ";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@delta", deltaQuantidade);
            comando.Parameters.AddWithValue("@id", idComponente);

            int resultado = await comando.ExecuteNonQueryAsync();

            return resultado > 0;
        }
    }
}