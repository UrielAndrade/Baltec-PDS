using Baltec.models;

using MySqlConnector;

namespace Baltec.DAO
{
    public class MovimentacaoEstoqueDAO
    {
        private readonly string connectionString;

        public MovimentacaoEstoqueDAO(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("String de conexão não encontrada.");
        }

        // Registra a movimentação e atualiza o estoque
        // dentro da mesma transação.
        public async Task<bool> RegistrarMovimentacaoAsync(
            MovimentacaoEstoque mov)
        {
            using var conexao = new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            using var transacao = await conexao.BeginTransactionAsync();

            try
            {
                // Verifica se é entrada ou saída.
                // 1 = entrada
                // 2 = saída
                // 3 = ajuste
                int delta = mov.Quantidade;

                if (mov.FkTipoMovimentacao == 2)
                {
                    delta = -mov.Quantidade;
                }

                // Atualiza o estoque
                string sqlEstoque = @"
                    UPDATE componente
                    SET quantidade_estoque = quantidade_estoque + @delta
                    WHERE id = @id
                      AND quantidade_estoque + @delta >= 0;
                ";

                using var comandoEstoque =
                    new MySqlCommand(sqlEstoque, conexao, transacao);

                comandoEstoque.Parameters.AddWithValue("@delta", delta);
                comandoEstoque.Parameters.AddWithValue("@id", mov.FkComponente);

                int estoqueAtualizado =
                    await comandoEstoque.ExecuteNonQueryAsync();

                if (estoqueAtualizado == 0)
                {
                    await transacao.RollbackAsync();
                    return false;
                }

                // Registra a movimentação
                string sqlMovimentacao = @"
                    INSERT INTO movimentacao_estoque
                    (
                        fk_componente,
                        fk_tipo_movimentacao,
                        motivo_movimentacao,
                        quantidade,
                        custo_unitario,
                        fk_fornecedor,
                        fk_ordem_servico,
                        fk_usuario_responsavel
                    )
                    VALUES
                    (
                        @componente,
                        @tipo,
                        @motivo,
                        @quantidade,
                        @custo,
                        @fornecedor,
                        @ordemServico,
                        @usuario
                    );
                ";

                using var comandoMovimentacao =
                    new MySqlCommand(sqlMovimentacao, conexao, transacao);

                comandoMovimentacao.Parameters.AddWithValue(
                    "@componente", mov.FkComponente);

                comandoMovimentacao.Parameters.AddWithValue(
                    "@tipo", mov.FkTipoMovimentacao);

                comandoMovimentacao.Parameters.AddWithValue(
                    "@motivo", mov.MotivoMovimentacao);

                comandoMovimentacao.Parameters.AddWithValue(
                    "@quantidade", mov.Quantidade);

                comandoMovimentacao.Parameters.AddWithValue(
                    "@custo", mov.CustoUnitario);

                comandoMovimentacao.Parameters.AddWithValue(
                    "@fornecedor",
                    mov.FkFornecedor.HasValue
                        ? mov.FkFornecedor.Value
                        : DBNull.Value);

                comandoMovimentacao.Parameters.AddWithValue(
                    "@ordemServico",
                    mov.FkOrdemServico.HasValue
                        ? mov.FkOrdemServico.Value
                        : DBNull.Value);

                comandoMovimentacao.Parameters.AddWithValue(
                    "@usuario", mov.FkUsuarioResponsavel);

                await comandoMovimentacao.ExecuteNonQueryAsync();

                await transacao.CommitAsync();

                return true;
            }
            catch
            {
                await transacao.RollbackAsync();

                return false;
            }
        }

        // Lista as últimas movimentações
        public async Task<List<MovimentacaoEstoque>>
            ListarUltimasMovimentacoesAsync(int limite = 50)
        {
            List<MovimentacaoEstoque> movimentacoes =
                new List<MovimentacaoEstoque>();

            using var conexao = new MySqlConnection(connectionString);

            await conexao.OpenAsync();

            string sql = @"
                SELECT
                    m.id,
                    m.fk_componente,
                    c.nome AS componente_nome,
                    m.fk_tipo_movimentacao,
                    t.nome AS tipo_movimentacao_nome,
                    m.motivo_movimentacao,
                    m.quantidade,
                    m.custo_unitario,
                    m.fk_fornecedor,
                    m.fk_ordem_servico,
                    m.fk_usuario_responsavel,
                    m.data_movimentacao
                FROM movimentacao_estoque m
                INNER JOIN componente c
                    ON c.id = m.fk_componente
                INNER JOIN tipo_movimentacao t
                    ON t.id = m.fk_tipo_movimentacao
                ORDER BY m.data_movimentacao DESC
                LIMIT @limite;
            ";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@limite", limite);

            using var reader = await comando.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                movimentacoes.Add(new MovimentacaoEstoque
                {
                    Id = Convert.ToInt32(reader["id"]),

                    FkComponente =
                        Convert.ToInt32(reader["fk_componente"]),

                    ComponenteNome =
                        reader["componente_nome"].ToString(),

                    FkTipoMovimentacao =
                        Convert.ToInt32(reader["fk_tipo_movimentacao"]),

                    TipoMovimentacaoNome =
                        reader["tipo_movimentacao_nome"].ToString(),

                    MotivoMovimentacao =
                        reader["motivo_movimentacao"].ToString() ?? "",

                    Quantidade =
                        Convert.ToInt32(reader["quantidade"]),

                    CustoUnitario =
                        Convert.ToDecimal(reader["custo_unitario"]),

                    FkFornecedor =
                        reader["fk_fornecedor"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["fk_fornecedor"]),

                    FkOrdemServico =
                        reader["fk_ordem_servico"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["fk_ordem_servico"]),

                    FkUsuarioResponsavel =
                        Convert.ToInt32(
                            reader["fk_usuario_responsavel"]),

                    DataMovimentacao =
                        Convert.ToDateTime(reader["data_movimentacao"])
                });
            }

            return movimentacoes;
        }
    }
}