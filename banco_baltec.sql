-- ============================================================================
-- PROJETO BALTEC - BANCO DE DADOS COMPLETO (MySQL Workbench Compatible)
-- ============================================================================

-- 1. CRIAÇÃO DO BANCO DE DADOS E DDL BÁSICO
DROP DATABASE IF EXISTS baltec_db;
CREATE DATABASE baltec_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE baltec_db;

-- ----------------------------------------------------------------------------
-- TABELA: Categorias de Componentes/Peças
-- ----------------------------------------------------------------------------
CREATE TABLE categorias (
    id_categoria INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(255)
);

-- ----------------------------------------------------------------------------
-- TABELA: Componentes / Peças de Estoque
-- ----------------------------------------------------------------------------
CREATE TABLE componentes (
    id_componente INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(120) NOT NULL,
    id_categoria INT NOT NULL,
    quantidade_estoque INT NOT NULL DEFAULT 0,
    preco_unitario DECIMAL(10, 2) NOT NULL,
    data_cadastro DATE NOT NULL,
    CONSTRAINT fk_componente_categoria FOREIGN KEY (id_categoria) REFERENCES categorias(id_categoria)
);

-- ----------------------------------------------------------------------------
-- TABELA: Pessoas (Modelo Conceitual: Generalização / Especialização)
-- ----------------------------------------------------------------------------
CREATE TABLE pessoas (
    id_pessoa INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(150) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    telefone VARCHAR(20),
    tipo_pessoa ENUM('CLIENTE', 'TECNICO') NOT NULL
);

-- Especialização: Clientes
CREATE TABLE clientes (
    id_pessoa INT PRIMARY KEY,
    cnpj_cpf VARCHAR(20) UNIQUE NOT NULL,
    endereco VARCHAR(255),
    CONSTRAINT fk_cliente_pessoa FOREIGN KEY (id_pessoa) REFERENCES pessoas(id_pessoa) ON DELETE CASCADE
);

-- Especialização: Técnicos
CREATE TABLE tecnicos (
    id_pessoa INT PRIMARY KEY,
    especialidade VARCHAR(100) NOT NULL,
    valor_hora DECIMAL(10,2) NOT NULL,
    CONSTRAINT fk_tecnico_pessoa FOREIGN KEY (id_pessoa) REFERENCES pessoas(id_pessoa) ON DELETE CASCADE
);

-- ----------------------------------------------------------------------------
-- TABELA: Balanças (Equipamentos)
-- ----------------------------------------------------------------------------
CREATE TABLE balancas (
    id_balanca INT AUTO_INCREMENT PRIMARY KEY,
    id_cliente INT NOT NULL,
    modelo VARCHAR(100) NOT NULL,
    numero_serie VARCHAR(50) UNIQUE NOT NULL,
    capacidade_max_kg DECIMAL(10,2) NOT NULL,
    precisao_g DECIMAL(8,3) NOT NULL,
    CONSTRAINT fk_balanca_cliente FOREIGN KEY (id_cliente) REFERENCES clientes(id_pessoa)
);

-- ----------------------------------------------------------------------------
-- TABELA: Ordens de Serviço (OS)
-- ----------------------------------------------------------------------------
CREATE TABLE ordens_servico (
    id_os INT AUTO_INCREMENT PRIMARY KEY,
    id_balanca INT NOT NULL,
    id_tecnico INT NOT NULL,
    data_abertura DATE NOT NULL,
    data_conclusao DATE,
    status ENUM('ABERTA', 'EM_ANDAMENTO', 'CONCLUIDA', 'CANCELADA') DEFAULT 'ABERTA',
    valor_mao_de_obra DECIMAL(10,2) DEFAULT 0.00,
    CONSTRAINT fk_os_balanca FOREIGN KEY (id_balanca) REFERENCES balancas(id_balanca),
    CONSTRAINT fk_os_tecnico FOREIGN KEY (id_tecnico) REFERENCES tecnicos(id_pessoa)
);

-- ----------------------------------------------------------------------------
-- TABELA N:M - Peças Utilizadas na Ordem de Serviço
-- ----------------------------------------------------------------------------
CREATE TABLE os_componentes (
    id_os INT NOT NULL,
    id_componente INT NOT NULL,
    quantidade INT NOT NULL,
    preco_aplicado DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (id_os, id_componente),
    CONSTRAINT fk_oscomp_os FOREIGN KEY (id_os) REFERENCES ordens_servico(id_os) ON DELETE CASCADE,
    CONSTRAINT fk_oscomp_componente FOREIGN KEY (id_componente) REFERENCES componentes(id_componente)
);

-- ----------------------------------------------------------------------------
-- TABELA: Certificados de Calibração
-- ----------------------------------------------------------------------------
CREATE TABLE certificados_calibracao (
    id_certificado INT AUTO_INCREMENT PRIMARY KEY,
    id_os INT UNIQUE NOT NULL,
    numero_certificado VARCHAR(50) UNIQUE NOT NULL,
    data_emissao DATE NOT NULL,
    resultado_aprovado BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT fk_cert_os FOREIGN KEY (id_os) REFERENCES ordens_servico(id_os)
);

-- ============================================================================
-- DEMONSTRAÇÃO DDL: ALTER TABLE e DROP TABLE (Exemplo praticado)
-- ============================================================================
-- Teste de alteração de estrutura
ALTER TABLE componentes ADD COLUMN observacao VARCHAR(255) NULL;
ALTER TABLE componentes DROP COLUMN observacao;

-- Criando e deletando uma tabela temporária para demonstrar o DROP TABLE
CREATE TABLE tabela_teste_drop (id INT PRIMARY KEY, nome VARCHAR(50));
DROP TABLE tabela_teste_drop;


-- ============================================================================
-- 4. INSERÇÃO DE DADOS (5 INSERTS EM CADA TABELA)
-- ============================================================================

-- 1. Categorias (5 registros)
INSERT INTO categorias (nome, descricao) VALUES
('Células de Carga', 'Sensores de peso e força para balanças industriais'),
('Indicadores Digitais', 'Módulos de exibição e processamento de peso'),
('Cabos e Conectores', 'Cabeamento para transmissão de sinal e energia'),
('Fontes e Placas', 'Fontes de alimentação e placas-mãe'),
('Baterias e Fonte', 'Baterias recarregáveis e fontes externas');

-- 2. Componentes (5 registros)
INSERT INTO componentes (nome, id_categoria, quantidade_estoque, preco_unitario, data_cadastro) VALUES
('Célula de Carga 50kg Inox', 1, 15, 250.00, '2026-01-10'),
('Indicador Digital Baltec Pro', 2, 8, 450.50, '2026-01-15'),
('Cabo Blindado 4 Vias (metro)', 3, 120, 12.00, '2026-02-01'),
('Placa Principal CPU V2', 4, 5, 320.00, '2026-02-10'),
('Bateria Selada 6V 4.5Ah', 5, 25, 85.90, '2026-03-01');

-- 3. Pessoas (10 registros: 5 Clientes e 5 Técnicos)
INSERT INTO pessoas (nome, email, telefone, tipo_pessoa) VALUES
('Indústria Alimentos Alfa', 'contato@alfaalimentos.com', '(11) 98888-1111', 'CLIENTE'),
('Logística Expressa Beta', 'suporte@betalog.com', '(11) 98888-2222', 'CLIENTE'),
('Supermercados Gama', 'compras@gamasuper.com', '(11) 98888-3333', 'CLIENTE'),
('Metalúrgica Delta', 'manutencao@deltametal.com', '(11) 98888-4444', 'CLIENTE'),
('Agrícola Epsilon', 'admin@epsilonagro.com', '(11) 98888-5555', 'CLIENTE'),
('Carlos Eduardo Silva', 'carlos.tecnico@baltec.com', '(11) 97777-1010', 'TECNICO'),
('Mariana Rocha', 'mariana.tecnica@baltec.com', '(11) 97777-2020', 'TECNICO'),
('Roberto Santos', 'roberto.tecnico@baltec.com', '(11) 97777-3030', 'TECNICO'),
('Fernanda Lima', 'fernanda.tecnica@baltec.com', '(11) 97777-4040', 'TECNICO'),
('Lucas Oliveira', 'lucas.tecnico@baltec.com', '(11) 97777-5050', 'TECNICO');

-- 4. Clientes (5 registros - IDs 1 a 5)
INSERT INTO clientes (id_pessoa, cnpj_cpf, endereco) VALUES
(1, '12.345.678/0001-90', 'Av. das Indústrias, 1000 - São Paulo/SP'),
(2, '98.765.432/0001-10', 'Rua dos Galpões, 500 - Guarulhos/SP'),
(3, '11.222.333/0001-44', 'Av. Paulista, 1500 - São Paulo/SP'),
(4, '55.666.777/0001-88', 'Rodovia Metalúrgicos, Km 12 - Osasco/SP'),
(5, '99.888.777/0001-22', 'Estrada Rural, s/n - Campinas/SP');

-- 5. Técnicos (5 registros - IDs 6 a 10)
INSERT INTO tecnicos (id_pessoa, especialidade, valor_hora) VALUES
(6, 'Calibração de Balanças Rodoviárias', 120.00),
(7, 'Manutenção de Microbalanças', 150.00),
(8, 'Ajuste de Células de Carga', 110.00),
(9, 'Manutenção de Indicadores Digitais', 130.00),
(10, 'Inspeção e Certificação Inmetro', 160.00);

-- 6. Balanças (5 registros)
INSERT INTO balancas (id_cliente, modelo, numero_serie, capacidade_max_kg, precisao_g) VALUES
(1, 'Toledo 2090', 'SN-2090-001', 500.00, 10.000),
(2, 'Filizola Platinum', 'SN-FPL-002', 30.00, 2.000),
(3, 'Urano Pop Light', 'SN-UPL-003', 15.00, 1.000),
(4, 'Micheletti Rodoviária', 'SN-MCH-004', 60000.00, 500.000),
(5, 'Gehaka BG2000', 'SN-GH-005', 2.00, 0.010);

-- 7. Ordens de Serviço (5 registros)
INSERT INTO ordens_servico (id_balanca, id_tecnico, data_abertura, data_conclusao, status, valor_mao_de_obra) VALUES
(1, 6, '2026-07-01', '2026-07-03', 'CONCLUIDA', 360.00),
(2, 7, '2026-07-10', '2026-07-12', 'CONCLUIDA', 300.00),
(3, 8, '2026-07-20', '2026-07-25', 'CONCLUIDA', 220.00),
(4, 9, '2026-08-01', NULL, 'EM_ANDAMENTO', 260.00),
(5, 10, '2026-08-03', NULL, 'ABERTA', 160.00);

-- 8. OS Componentes (5 registros)
INSERT INTO os_componentes (id_os, id_componente, quantidade, preco_aplicado) VALUES
(1, 1, 2, 250.00),
(1, 3, 5, 12.00),
(2, 5, 1, 85.90),
(3, 2, 1, 450.50),
(4, 4, 1, 320.00);

-- 9. Certificados de Calibração (5 registros)
INSERT INTO certificados_calibracao (id_os, numero_certificado, data_emissao, resultado_aprovado) VALUES
(1, 'CERT-2026-001', '2026-07-03', TRUE),
(2, 'CERT-2026-002', '2026-07-12', TRUE),
(3, 'CERT-2026-003', '2026-07-25', TRUE),
(4, 'CERT-2026-004', '2026-08-02', FALSE),
(5, 'CERT-2026-005', '2026-08-04', TRUE);


-- ============================================================================
-- 5. COMANDOS DML DE MODIFICAÇÃO (UPDATE e DELETE)
-- ============================================================================

-- Atualizando preço e quantidade no estoque utilizando a cláusula WHERE
UPDATE componentes 
SET quantidade_estoque = quantidade_estoque + 10, preco_unitario = 260.00 
WHERE id_componente = 1;

-- Atualizando status de Ordem de Serviço
UPDATE ordens_servico 
SET status = 'CONCLUIDA', data_conclusao = CURDATE() 
WHERE id_os = 4;

-- Exemplo de DELETE (Deletando registro específico usando WHERE)
-- Insere um registro temporário para exclusão:
INSERT INTO categorias (nome, descricao) VALUES ('Temp Categoria', 'Para excluir');
DELETE FROM categorias WHERE nome = 'Temp Categoria';


-- ============================================================================
-- 6. CONSULTAS COM WHERE, OPERADORES, LIKE, BETWEEN, ORDER BY E GROUP BY
-- ============================================================================

-- Operadores de Comparação (=, >, <, >=, <=, <>)
SELECT * FROM componentes WHERE preco_unitario >= 100.00;
SELECT * FROM ordens_servico WHERE status <> 'CANCELADA';

-- Operador LIKE (Busca por padrões de texto)
SELECT * FROM pessoas WHERE nome LIKE 'Indústria%';
SELECT * FROM balancas WHERE numero_serie LIKE '%FPL%';

-- Operador BETWEEN (Busca em faixa de valores ou datas)
SELECT * FROM componentes WHERE preco_unitario BETWEEN 50.00 AND 300.00;
SELECT * FROM ordens_servico WHERE data_abertura BETWEEN '2026-07-01' AND '2026-07-31';

-- CONSULTA COM ORDER BY E GROUP BY
SELECT id_categoria, COUNT(*) AS total_itens, AVG(preco_unitario) AS media_preco
FROM componentes
GROUP BY id_categoria
ORDER BY media_preco DESC;


-- ============================================================================
-- 7. FUNÇÕES DE AGREGAÇÃO (MAX, MIN, SUM, COUNT, AVG)
-- ============================================================================
SELECT 
    COUNT(*) AS total_componentes,
    MAX(preco_unitario) AS maior_preco,
    MIN(preco_unitario) AS menor_preco,
    SUM(quantidade_estoque) AS estoque_total_pecas,
    AVG(preco_unitario) AS preco_medio
FROM componentes;


-- ============================================================================
-- 8. FUNÇÕES DO MYSQL (CURDATE, DATEDIFF, CONCAT, UPPER, etc.)
-- ============================================================================
SELECT 
    id_os,
    data_abertura,
    data_conclusao,
    CURDATE() AS data_atual,
    DATEDIFF(IFNULL(data_conclusao, CURDATE()), data_abertura) AS dias_em_atendimento,
    UPPER(status) AS status_formatado
FROM ordens_servico;


-- ============================================================================
-- 9 & 10. VISÕES (VIEWS)
-- ============================================================================

-- Criação da View: Relatório completo de Ordens de Serviço
CREATE OR REPLACE VIEW vw_detalhes_ordem_servico AS
SELECT 
    os.id_os,
    cli_p.nome AS cliente,
    tec_p.nome AS tecnico,
    b.modelo AS modelo_balanca,
    b.numero_serie,
    os.data_abertura,
    os.data_conclusao,
    os.status,
    os.valor_mao_de_obra,
    DATEDIFF(IFNULL(os.data_conclusao, CURDATE()), os.data_abertura) AS dias_aberto
FROM ordens_servico os
JOIN balancas b ON os.id_balanca = b.id_balanca
JOIN clientes c ON b.id_cliente = c.id_pessoa
JOIN pessoas cli_p ON c.id_pessoa = cli_p.id_pessoa
JOIN tecnicos t ON os.id_tecnico = t.id_pessoa
JOIN pessoas tec_p ON t.id_pessoa = tec_p.id_pessoa;

-- Consulta utilizando a View com filtros (SELECT Avançado)
SELECT * 
FROM vw_detalhes_ordem_servico 
WHERE status = 'CONCLUIDA'
ORDER BY dias_aberto DESC;
