-- ============================================================================
-- BALTEC - BANCO DE DADOS OFICIAL (MySQL 8.0 / MariaDB)
-- Compatível com Docker, XAMPP, phpMyAdmin, MySQL Workbench e DBeaver
-- ============================================================================

CREATE DATABASE IF NOT EXISTS baltec
    DEFAULT CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE baltec;

-- ----------------------------------------------------------------------------
-- 1. TABELAS DE DOMÍNIO / LOOKUP
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS cargo (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL UNIQUE,
    descricao VARCHAR(255) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS perfil (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE,
    descricao VARCHAR(255) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS permissao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL UNIQUE,
    descricao VARCHAR(255) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS perfil_permissao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_perfil INT NOT NULL,
    fk_permissao INT NOT NULL,
    FOREIGN KEY (fk_perfil) REFERENCES perfil(id) ON DELETE CASCADE,
    FOREIGN KEY (fk_permissao) REFERENCES permissao(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS categoria_componente (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS tipo_servico (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS status_os (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS grau_urgencia (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS tipo_movimentacao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS tipo_transacao_financeira (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS status_transacao_financeira (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS configuracao_sistema (
    id INT AUTO_INCREMENT PRIMARY KEY,
    chave VARCHAR(100) NOT NULL UNIQUE,
    valor TEXT NOT NULL,
    descricao VARCHAR(255) NULL,
    data_atualizacao DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 2. USUÁRIOS E PERMISSÕES
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS usuario (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome_completo VARCHAR(150) NOT NULL,
    cpf VARCHAR(14) NOT NULL UNIQUE,
    telefone VARCHAR(15) NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    fk_cargo INT NOT NULL,
    senha_hash VARCHAR(255) NOT NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
    ativo BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (fk_cargo) REFERENCES cargo(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS usuario_perfil (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_usuario INT NOT NULL,
    fk_perfil INT NOT NULL,
    FOREIGN KEY (fk_usuario) REFERENCES usuario(id) ON DELETE CASCADE,
    FOREIGN KEY (fk_perfil) REFERENCES perfil(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 3. CLIENTES E FORNECEDORES
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS cliente (
    id INT AUTO_INCREMENT PRIMARY KEY,
    razao_social VARCHAR(150) NOT NULL,
    nome_fantasia VARCHAR(150) NULL,
    cnpj_cpf VARCHAR(18) NOT NULL UNIQUE,
    telefone VARCHAR(15) NULL,
    email VARCHAR(100) NULL,
    endereco VARCHAR(255) NULL,
    cidade VARCHAR(100) NULL,
    estado VARCHAR(2) NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
    ativo BOOLEAN DEFAULT TRUE,
    INDEX idx_cliente_doc (cnpj_cpf),
    INDEX idx_cliente_razao (razao_social)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS fornecedor (
    id INT AUTO_INCREMENT PRIMARY KEY,
    razao_social VARCHAR(150) NOT NULL,
    nome_fantasia VARCHAR(150) NULL,
    cnpj_cpf VARCHAR(18) NOT NULL UNIQUE,
    telefone VARCHAR(15) NULL,
    email VARCHAR(100) NULL,
    endereco VARCHAR(255) NULL,
    cidade VARCHAR(100) NULL,
    estado VARCHAR(2) NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
    ativo BOOLEAN DEFAULT TRUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 4. EQUIPAMENTOS (BALANÇAS) E COMPONENTES
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS equipamento (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_cliente INT NOT NULL,
    modelo VARCHAR(100) NOT NULL,
    marca_fabricante VARCHAR(100) NOT NULL,
    capacidade_maxima_kg DECIMAL(10,3) NOT NULL,
    divisao_escala_g DECIMAL(10,3) NOT NULL,
    numero_serie VARCHAR(50) NOT NULL UNIQUE,
    setor_localizacao VARCHAR(100) NOT NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
    ativo BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (fk_cliente) REFERENCES cliente(id),
    INDEX idx_equip_serie (numero_serie),
    INDEX idx_equip_cliente (fk_cliente)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS componente (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(150) NOT NULL,
    codigo_item VARCHAR(50) NOT NULL UNIQUE,
    fk_categoria INT NOT NULL,
    quantidade_estoque INT NOT NULL DEFAULT 0,
    preco_unitario DECIMAL(18,2) NOT NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
    ativo BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (fk_categoria) REFERENCES categoria_componente(id),
    INDEX idx_comp_codigo (codigo_item)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 5. ORDENS DE SERVIÇO E SOLICITAÇÕES DE PEÇAS
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS ordem_servico (
    id INT AUTO_INCREMENT PRIMARY KEY,
    numero_os VARCHAR(50) NOT NULL UNIQUE,
    fk_cliente INT NOT NULL,
    fk_equipamento INT NOT NULL,
    fk_tipo_servico INT NOT NULL,
    fk_tecnico INT NOT NULL,
    descricao_problema TEXT NOT NULL,
    fk_status INT NOT NULL,
    data_abertura DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_conclusao DATETIME NULL,
    FOREIGN KEY (fk_cliente) REFERENCES cliente(id),
    FOREIGN KEY (fk_equipamento) REFERENCES equipamento(id),
    FOREIGN KEY (fk_tipo_servico) REFERENCES tipo_servico(id),
    FOREIGN KEY (fk_tecnico) REFERENCES usuario(id),
    FOREIGN KEY (fk_status) REFERENCES status_os(id),
    INDEX idx_os_numero (numero_os),
    INDEX idx_os_status (fk_status),
    INDEX idx_os_data (data_abertura)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS ordem_servico_peca (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_ordem_servico_principal INT NULL,
    fk_componente INT NOT NULL,
    quantidade INT NOT NULL,
    fk_urgencia INT NOT NULL,
    observacoes TEXT NOT NULL,
    fk_status INT NOT NULL,
    data_solicitacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_resolucao DATETIME NULL,
    FOREIGN KEY (fk_ordem_servico_principal) REFERENCES ordem_servico(id) ON DELETE SET NULL,
    FOREIGN KEY (fk_componente) REFERENCES componente(id),
    FOREIGN KEY (fk_urgencia) REFERENCES grau_urgencia(id),
    FOREIGN KEY (fk_status) REFERENCES status_os(id),
    INDEX idx_osp_os (fk_ordem_servico_principal),
    INDEX idx_osp_status (fk_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 6. CERTIFICADOS DE CALIBRAÇÃO (METROLOGIA RBC)
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS certificado_calibracao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    numero_certificado VARCHAR(50) NOT NULL UNIQUE,
    fk_ordem_servico INT NULL,
    fk_equipamento INT NOT NULL,
    data_calibracao DATE NOT NULL,
    data_proxima_calibracao DATE NOT NULL,
    temperatura_ambiente DECIMAL(5,2) NOT NULL,
    umidade_relativa DECIMAL(5,2) NOT NULL,
    fk_tecnico_responsavel INT NOT NULL,
    data_emissao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (fk_ordem_servico) REFERENCES ordem_servico(id) ON DELETE SET NULL,
    FOREIGN KEY (fk_equipamento) REFERENCES equipamento(id),
    FOREIGN KEY (fk_tecnico_responsavel) REFERENCES usuario(id),
    INDEX idx_cert_numero (numero_certificado),
    INDEX idx_cert_proxima (data_proxima_calibracao)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 7. MOVIMENTAÇÃO DE ESTOQUE
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS movimentacao_estoque (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_componente INT NOT NULL,
    fk_tipo_movimentacao INT NOT NULL,
    motivo_movimentacao VARCHAR(150) NOT NULL,
    quantidade INT NOT NULL,
    custo_unitario DECIMAL(18,2) NOT NULL,
    fk_fornecedor INT NULL,
    fk_ordem_servico INT NULL,
    fk_usuario_responsavel INT NOT NULL,
    data_movimentacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (fk_componente) REFERENCES componente(id),
    FOREIGN KEY (fk_tipo_movimentacao) REFERENCES tipo_movimentacao(id),
    FOREIGN KEY (fk_fornecedor) REFERENCES fornecedor(id) ON DELETE SET NULL,
    FOREIGN KEY (fk_ordem_servico) REFERENCES ordem_servico(id) ON DELETE SET NULL,
    FOREIGN KEY (fk_usuario_responsavel) REFERENCES usuario(id),
    INDEX idx_mov_comp (fk_componente),
    INDEX idx_mov_data (data_movimentacao)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 8. MÓDULO FINANCEIRO (FLUXO DE CAIXA, CONTAS A PAGAR E A RECEBER)
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS transacao_financeira (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_tipo_transacao INT NOT NULL,
    descricao VARCHAR(255) NOT NULL,
    valor DECIMAL(18,2) NOT NULL,
    fk_status INT NOT NULL,
    data_vencimento DATE NOT NULL,
    data_pagamento DATE NULL,
    fk_cliente INT NULL,
    fk_fornecedor INT NULL,
    fk_ordem_servico INT NULL,
    data_cadastro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (fk_tipo_transacao) REFERENCES tipo_transacao_financeira(id),
    FOREIGN KEY (fk_status) REFERENCES status_transacao_financeira(id),
    FOREIGN KEY (fk_cliente) REFERENCES cliente(id) ON DELETE SET NULL,
    FOREIGN KEY (fk_fornecedor) REFERENCES fornecedor(id) ON DELETE SET NULL,
    FOREIGN KEY (fk_ordem_servico) REFERENCES ordem_servico(id) ON DELETE SET NULL,
    INDEX idx_trans_vencimento (data_vencimento),
    INDEX idx_trans_tipo (fk_tipo_transacao),
    INDEX idx_trans_status (fk_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 9. AUDITORIA E LOG
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS auditoria_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tabela_afetada VARCHAR(100) NOT NULL,
    registro_id INT NOT NULL,
    acao VARCHAR(20) NOT NULL,
    dados_antigos TEXT NULL,
    dados_novos TEXT NULL,
    fk_usuario INT NULL,
    data_hora DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (fk_usuario) REFERENCES usuario(id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- ============================================================================
-- DADOS INICIAIS (SEEDS) DO SISTEMA
-- ============================================================================

-- Cargos
INSERT IGNORE INTO cargo (id, nome, descricao) VALUES 
(1, 'Técnico de Calibração', 'Técnico responsável pelas aferições e emissão de laudos'),
(2, 'Engenheiro', 'Engenheiro metrologista e responsável técnico'),
(3, 'Administrativo', 'Equipe de escritório, compras e financeiro'),
(4, 'Gerente', 'Gerente da unidade operacional');

-- Perfis de Acesso
INSERT IGNORE INTO perfil (id, nome, descricao) VALUES 
(1, 'Admin', 'Acesso total e configurações do sistema'),
(2, 'Tecnico', 'Acesso operacional a ordens de serviço, peças e calibrações'),
(3, 'Atendimento', 'Acesso a cadastros de clientes, balanças e financeiro');

-- Tipos de Serviço de OS
INSERT IGNORE INTO tipo_servico (id, nome) VALUES 
(1, 'Calibração de Balança Analítica'),
(2, 'Manutenção Preventiva'),
(3, 'Manutenção Corretiva'),
(4, 'Aferição de Padrões');

-- Status de Ordem de Serviço
INSERT IGNORE INTO status_os (id, nome) VALUES 
(1, 'Pendente'),
(2, 'Em Andamento'),
(3, 'Aguardando Peça'),
(4, 'Concluído'),
(5, 'Cancelado');

-- Graus de Urgência
INSERT IGNORE INTO grau_urgencia (id, nome) VALUES 
(1, 'Baixa'),
(2, 'Média'),
(3, 'Alta'),
(4, 'Crítica');

-- Tipos de Movimentação de Estoque
INSERT IGNORE INTO tipo_movimentacao (id, nome) VALUES 
(1, 'Entrada (Compra)'),
(2, 'Saída (OS)'),
(3, 'Ajuste de Estoque (+/-)');

-- Categorias de Componentes e Peças
INSERT IGNORE INTO categoria_componente (id, nome) VALUES 
(1, 'Sensores e Células de Carga'),
(2, 'Placas de Circuito e Fontes'),
(3, 'Cabos e Conectores'),
(4, 'Pés e Amortecedores'),
(5, 'Displays e Teclados');

-- Tipos e Status de Transação Financeira
INSERT IGNORE INTO tipo_transacao_financeira (id, nome) VALUES 
(1, 'Receita'),
(2, 'Despesa');

INSERT IGNORE INTO status_transacao_financeira (id, nome) VALUES 
(1, 'Pendente'),
(2, 'Pago'),
(3, 'Atrasado'),
(4, 'Cancelado');

-- ----------------------------------------------------------------------------
-- SEEDS: USUÁRIOS E TÉCNICOS
-- Senha padrão para todos os usuários: 123456
-- ----------------------------------------------------------------------------
INSERT IGNORE INTO usuario (id, nome_completo, cpf, telefone, email, fk_cargo, senha_hash, data_cadastro, ativo) VALUES 
(1, 'Administrador Baltec', '000.000.000-00', '(11) 99999-0001', 'admin@baltec.com.br', 4, '$2a$11$qR6m.Fk7m2cZ9sO4y7XbCeV5d3A4Z8a9B0c1D2e3F4g5H6i7J8k9L', NOW(), 1),
(2, 'Roberto Ramos - Metrologista', '111.222.333-44', '(11) 98888-2222', 'roberto@baltec.com.br', 1, '$2a$11$qR6m.Fk7m2cZ9sO4y7XbCeV5d3A4Z8a9B0c1D2e3F4g5H6i7J8k9L', NOW(), 1),
(3, 'Carlos Eduardo - Técnico Especialista', '222.333.444-55', '(19) 97777-3333', 'carlos@baltec.com.br', 1, '$2a$11$qR6m.Fk7m2cZ9sO4y7XbCeV5d3A4Z8a9B0c1D2e3F4g5H6i7J8k9L', NOW(), 1),
(4, 'Eng. Marcos Oliveira', '333.444.555-66', '(16) 96666-4444', 'marcos@baltec.com.br', 2, '$2a$11$qR6m.Fk7m2cZ9sO4y7XbCeV5d3A4Z8a9B0c1D2e3F4g5H6i7J8k9L', NOW(), 1);

INSERT IGNORE INTO usuario_perfil (fk_usuario, fk_perfil) VALUES 
(1, 1),
(2, 2),
(3, 2),
(4, 2);

-- ----------------------------------------------------------------------------
-- SEEDS: CLIENTES
-- ----------------------------------------------------------------------------
INSERT IGNORE INTO cliente (id, razao_social, nome_fantasia, cnpj_cpf, telefone, email, endereco, cidade, estado, data_cadastro, ativo) VALUES 
(1, 'Empresa Alpha Ltda', 'Alpha Indústria & Comércio', '12.345.678/0001-90', '(11) 98888-1111', 'contato@alpha.com.br', 'Av. Industrial, 1000', 'São Paulo', 'SP', NOW(), 1),
(2, 'Indústrias Beta S/A', 'Beta Alimentos', '98.765.432/0001-10', '(19) 97777-2222', 'operacoes@beta.com.br', 'Rodovia Santos Dumont, km 45', 'Campinas', 'SP', NOW(), 1),
(3, 'Frigorífico Sigma Carnes', 'Sigma Carnes', '45.123.789/0001-55', '(16) 96666-3333', 'manutencao@sigma.com', 'Distrito Agroindustrial, s/n', 'Ribeirão Preto', 'SP', NOW(), 1),
(4, 'Metalúrgica Delta Inox', 'Delta Inox', '33.654.987/0001-22', '(11) 95555-4444', 'compras@deltainox.com', 'Rua dos Metalúrgicos, 500', 'Guarulhos', 'SP', NOW(), 1);

-- ----------------------------------------------------------------------------
-- SEEDS: FORNECEDORES
-- ----------------------------------------------------------------------------
INSERT IGNORE INTO fornecedor (id, razao_social, nome_fantasia, cnpj_cpf, telefone, email, endereco, cidade, estado, data_cadastro, ativo) VALUES 
(1, 'Toledo do Brasil Indústria de Balanças Ltda', 'Toledo do Brasil', '60.450.412/0001-05', '(11) 4356-9000', 'vendas@toledobrasil.com', 'Rua Toledo, 200', 'São Bernardo do Campo', 'SP', NOW(), 1),
(2, 'Sensortec Componentes Industriais Ltda', 'Sensortec', '11.222.333/0001-44', '(11) 3222-1111', 'contato@sensortec.com.br', 'Av. Senador Vergueiro, 800', 'São Paulo', 'SP', NOW(), 1),
(3, 'Labmet Metrologia Acreditada Ltda', 'Labmet RBC', '22.333.444/0001-55', '(19) 3888-4444', 'atendimento@labmet.com.br', 'Rua das Aferições, 150', 'Campinas', 'SP', NOW(), 1);

-- ----------------------------------------------------------------------------
-- SEEDS: EQUIPAMENTOS (BALANÇAS)
-- ----------------------------------------------------------------------------
INSERT IGNORE INTO equipamento (id, fk_cliente, modelo, marca_fabricante, capacidade_maxima_kg, divisao_escala_g, numero_serie, setor_localizacao, data_cadastro, ativo) VALUES 
(1, 1, 'Toledo 2090', 'Toledo do Brasil', 300.000, 50.000, 'TOL-994821', 'Expedição', NOW(), 1),
(2, 1, 'Balança Analítica AS 220.R2', 'Radwag', 0.220, 0.0001, 'RDW-112344', 'Laboratório de Controle', NOW(), 1),
(3, 2, 'Filizola Platinum 30kg', 'Filizola', 30.000, 5.000, 'FLZ-883912', 'Linha de Montagem 02', NOW(), 1),
(4, 2, 'Balança de Piso 2000kg', 'Toledo', 2000.000, 500.000, 'TOL-556102', 'Almoxarifado Central', NOW(), 1),
(5, 3, 'Balança Rodoviária 80t', 'Baltec Precision', 80000.000, 10000.000, 'BLT-774901', 'Portaria Principal', NOW(), 1),
(6, 4, 'Balança Contadora Toledo 9094', 'Toledo', 15.000, 1.000, 'TOL-332110', 'Usinagem', NOW(), 1);

-- ----------------------------------------------------------------------------
-- SEEDS: COMPONENTES E PEÇAS DE ESTOQUE
-- ----------------------------------------------------------------------------
INSERT IGNORE INTO componente (id, nome, codigo_item, fk_categoria, quantidade_estoque, preco_unitario, data_cadastro, ativo) VALUES 
(1, 'Célula de Carga 50kg Inox', 'CEL-CARGA-50K', 1, 15, 450.00, NOW(), 1),
(2, 'Célula de Carga 500kg Kratos', 'CEL-CARGA-500K', 1, 8, 850.00, NOW(), 1),
(3, 'Placa Fonte de Alimentação 12V 2A', 'PLC-FONTE-12V', 2, 12, 280.00, NOW(), 1),
(4, 'Display LCD Backlight Azul 6 Dígitos', 'DISP-LCD-BL', 5, 20, 190.00, NOW(), 1),
(5, 'Teclado de Membrana Toledo 2090', 'TEC-MEMB-TOL', 5, 25, 120.00, NOW(), 1),
(6, 'Cabo Blindado Especial 4 Vias (Rolo 10m)', 'CAB-BLIND-4V', 3, 30, 85.00, NOW(), 1),
(7, 'Pé Nivelador de Borracha M12 com Sapata', 'PE-NIVEL-M12', 4, 50, 25.00, NOW(), 1);

-- ----------------------------------------------------------------------------
-- SEEDS: ORDENS DE SERVIÇO
-- ----------------------------------------------------------------------------
INSERT IGNORE INTO ordem_servico (id, numero_os, fk_cliente, fk_equipamento, fk_tipo_servico, fk_tecnico, descricao_problema, fk_status, data_abertura, data_conclusao) VALUES 
(1, 'OS-2026-0001', 1, 1, 2, 2, 'Limpeza preventiva nas células de carga e verificação do cabeamento de comunicação.', 4, NOW() - INTERVAL 10 DAY, NOW() - INTERVAL 8 DAY),
(2, 'OS-2026-0002', 2, 3, 3, 3, 'Balança apresentando oscilação na pesagem acima de 15kg e travamento no display.', 2, NOW() - INTERVAL 3 DAY, NULL),
(3, 'OS-2026-0003', 3, 5, 1, 4, 'Calibração metrológica anual obrigatória da balança rodoviária com pesos-padrão RBC.', 1, NOW() - INTERVAL 1 DAY, NULL);

-- ----------------------------------------------------------------------------
-- SEEDS: SOLICITAÇÕES DE PEÇAS DE OS
-- ----------------------------------------------------------------------------
INSERT IGNORE INTO ordem_servico_peca (id, fk_ordem_servico_principal, fk_componente, quantidade, fk_urgencia, observacoes, fk_status, data_solicitacao, data_resolucao) VALUES 
(1, 2, 1, 1, 3, 'Necessária substituição da célula de carga 50kg avariada por sobrecarga mecânica na linha 02.', 2, NOW() - INTERVAL 2 DAY, NULL);

-- ----------------------------------------------------------------------------
-- SEEDS: CERTIFICADOS DE CALIBRAÇÃO
-- ----------------------------------------------------------------------------
INSERT IGNORE INTO certificado_calibracao (id, numero_certificado, fk_ordem_servico, fk_equipamento, data_calibracao, data_proxima_calibracao, temperatura_ambiente, umidade_relativa, fk_tecnico_responsavel, data_emissao) VALUES 
(1, 'CERT-2026-0001', 1, 1, CURDATE() - INTERVAL 8 DAY, CURDATE() + INTERVAL 357 DAY, 22.50, 55.00, 2, NOW() - INTERVAL 8 DAY);

-- ----------------------------------------------------------------------------
-- SEEDS: MOVIMENTAÇÕES DE ESTOQUE
-- ----------------------------------------------------------------------------
INSERT IGNORE INTO movimentacao_estoque (id, fk_componente, fk_tipo_movimentacao, motivo_movimentacao, quantidade, custo_unitario, fk_fornecedor, fk_ordem_servico, fk_usuario_responsavel, data_movimentacao) VALUES 
(1, 1, 1, 'Entrada de reposição de estoque NF-4421', 10, 450.00, 2, NULL, 1, NOW() - INTERVAL 15 DAY),
(2, 5, 1, 'Compra de teclados de reposição Toledo', 15, 120.00, 1, NULL, 1, NOW() - INTERVAL 12 DAY);

-- ----------------------------------------------------------------------------
-- SEEDS: TRANSAÇÕES FINANCEIRAS
-- ----------------------------------------------------------------------------
INSERT IGNORE INTO transacao_financeira (id, fk_tipo_transacao, descricao, valor, fk_status, data_vencimento, data_pagamento, fk_cliente, fk_fornecedor, fk_ordem_servico, data_cadastro) VALUES 
(1, 1, 'Manutenção Preventiva e Aferição - Empresa Alpha', 1250.00, 2, CURDATE() - INTERVAL 5 DAY, CURDATE() - INTERVAL 5 DAY, 1, NULL, 1, NOW() - INTERVAL 8 DAY),
(2, 1, 'Calibração Metrológica Anual Balança 80t - Frigorífico Sigma', 4800.00, 1, CURDATE() + INTERVAL 15 DAY, NULL, 3, NULL, 3, NOW() - INTERVAL 1 DAY),
(3, 2, 'Aquisição de Componentes e Células de Carga - Sensortec', 2700.00, 2, CURDATE() - INTERVAL 10 DAY, CURDATE() - INTERVAL 10 DAY, NULL, 2, NULL, NOW() - INTERVAL 15 DAY),
(4, 2, 'Calibração Externa de Pesos-Padrão RBC / Inmetro - Labmet', 1850.00, 1, CURDATE() + INTERVAL 10 DAY, NULL, NULL, 3, NULL, NOW() - INTERVAL 2 DAY);
