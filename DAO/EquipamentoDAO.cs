using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO
{
    public class EquipamentoDAO
    {
        private readonly string connectionString =
            "Server=localhost;Database=baltec;Uid=root;Pwd=;Connection Timeout=1;";

        public async Task<List<Equipamento>> ListarTodosComClienteAsync()
        {
            var equipamentos = new List<Equipamento>();

            using var conexao = new MySqlConnection(connectionString);
            await conexao.OpenAsync();

            string sql = @"SELECT
                            e.id,
                            e.fk_cliente,
                            c.razao_social AS cliente_nome,
                            e.modelo,
                            e.marca_fabricante,
                            e.capacidade_maxima_kg,
                            e.divisao_escala_g,
                            e.numero_serie,
                            e.setor_localizacao,
                            e.data_cadastro,
                            e.ativo
                           FROM equipamento e
                           INNER JOIN cliente c ON e.fk_cliente = c.id
                           ORDER BY e.id";

            using var comando = new MySqlCommand(sql, conexao);
            using var reader = await comando.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                equipamentos.Add(MapearEquipamento(reader));
            }

            return equipamentos;
        }

        public async Task<List<Equipamento>> ListarPorClienteAsync(int idCliente)
        {
            var equipamentos = new List<Equipamento>();

            using var conexao = new MySqlConnection(connectionString);
            await conexao.OpenAsync();

            string sql = @"SELECT
                            e.id,
                            e.fk_cliente,
                            c.razao_social AS cliente_nome,
                            e.modelo,
                            e.marca_fabricante,
                            e.capacidade_maxima_kg,
                            e.divisao_escala_g,
                            e.numero_serie,
                            e.setor_localizacao,
                            e.data_cadastro,
                            e.ativo
                           FROM equipamento e
                           INNER JOIN cliente c ON e.fk_cliente = c.id
                           WHERE e.fk_cliente = @idCliente
                           ORDER BY e.id";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.AddWithValue("@idCliente", idCliente);

            using var reader = await comando.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                equipamentos.Add(MapearEquipamento(reader));
            }

            return equipamentos;
        }

        public async Task<Equipamento?> BuscarPorNumeroSerieAsync(string numSerie)
        {
            using var conexao = new MySqlConnection(connectionString);
            await conexao.OpenAsync();

            string sql = @"SELECT
                            e.id,
                            e.fk_cliente,
                            c.razao_social AS cliente_nome,
                            e.modelo,
                            e.marca_fabricante,
                            e.capacidade_maxima_kg,
                            e.divisao_escala_g,
                            e.numero_serie,
                            e.setor_localizacao,
                            e.data_cadastro,
                            e.ativo
                           FROM equipamento e
                           INNER JOIN cliente c ON e.fk_cliente = c.id
                           WHERE e.numero_serie = @numSerie";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.AddWithValue("@numSerie", numSerie);

            using var reader = await comando.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapearEquipamento(reader);
            }

            return null;
        }

        public async Task<bool> InserirAsync(Equipamento equipamento)
        {
            using var conexao = new MySqlConnection(connectionString);
            await conexao.OpenAsync();

            string sql = @"INSERT INTO equipamento
                           (
                               fk_cliente,
                               modelo,
                               marca_fabricante,
                               capacidade_maxima_kg,
                               divisao_escala_g,
                               numero_serie,
                               setor_localizacao,
                               ativo
                           )
                           VALUES
                           (
                               @fk_cliente,
                               @modelo,
                               @marca_fabricante,
                               @capacidade_maxima_kg,
                               @divisao_escala_g,
                               @numero_serie,
                               @setor_localizacao,
                               @ativo
                           )";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@fk_cliente", equipamento.FkCliente);
            comando.Parameters.AddWithValue("@modelo", equipamento.Modelo);
            comando.Parameters.AddWithValue("@marca_fabricante", equipamento.MarcaFabricante);
            comando.Parameters.AddWithValue("@capacidade_maxima_kg", equipamento.CapacidadeMaximaKg);
            comando.Parameters.AddWithValue("@divisao_escala_g", equipamento.DivisaoEscalaG);
            comando.Parameters.AddWithValue("@numero_serie", equipamento.NumeroSerie);
            comando.Parameters.AddWithValue("@setor_localizacao", equipamento.SetorLocalizacao);
            comando.Parameters.AddWithValue("@ativo", equipamento.Ativo);

            return await comando.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> AtualizarAsync(Equipamento equipamento)
        {
            using var conexao = new MySqlConnection(connectionString);
            await conexao.OpenAsync();

            string sql = @"UPDATE equipamento SET
                            fk_cliente = @fk_cliente,
                            modelo = @modelo,
                            marca_fabricante = @marca_fabricante,
                            capacidade_maxima_kg = @capacidade_maxima_kg,
                            divisao_escala_g = @divisao_escala_g,
                            numero_serie = @numero_serie,
                            setor_localizacao = @setor_localizacao,
                            ativo = @ativo
                           WHERE id = @id";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@id", equipamento.Id);
            comando.Parameters.AddWithValue("@fk_cliente", equipamento.FkCliente);
            comando.Parameters.AddWithValue("@modelo", equipamento.Modelo);
            comando.Parameters.AddWithValue("@marca_fabricante", equipamento.MarcaFabricante);
            comando.Parameters.AddWithValue("@capacidade_maxima_kg", equipamento.CapacidadeMaximaKg);
            comando.Parameters.AddWithValue("@divisao_escala_g", equipamento.DivisaoEscalaG);
            comando.Parameters.AddWithValue("@numero_serie", equipamento.NumeroSerie);
            comando.Parameters.AddWithValue("@setor_localizacao", equipamento.SetorLocalizacao);
            comando.Parameters.AddWithValue("@ativo", equipamento.Ativo);

            return await comando.ExecuteNonQueryAsync() > 0;
        }

        private Equipamento MapearEquipamento(MySqlDataReader reader)
        {
            return new Equipamento
            {
                Id = reader.GetInt32("id"),
                FkCliente = reader.GetInt32("fk_cliente"),
                ClienteNome = reader.IsDBNull(reader.GetOrdinal("cliente_nome"))
                    ? null
                    : reader.GetString("cliente_nome"),
                Modelo = reader.GetString("modelo"),
                MarcaFabricante = reader.GetString("marca_fabricante"),
                CapacidadeMaximaKg = reader.GetDecimal("capacidade_maxima_kg"),
                DivisaoEscalaG = reader.GetDecimal("divisao_escala_g"),
                NumeroSerie = reader.GetString("numero_serie"),
                SetorLocalizacao = reader.GetString("setor_localizacao"),
                DataCadastro = reader.GetDateTime("data_cadastro"),
                Ativo = reader.GetBoolean("ativo")
            };
        }
    }
}