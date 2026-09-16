create database baltec;

CREATE TABLE cargo (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(255) NULL
);

CREATE TABLE perfil (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE,
    descricao VARCHAR(255) NULL
);

CREATE TABLE permissao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL UNIQUE,
    descricao VARCHAR(255) NULL
);

CREATE TABLE perfil_permissao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_perfil INT NOT NULL,
    fk_permissao INT NOT NULL,
    FOREIGN KEY (fk_perfil) REFERENCEs perfil(id),
    FOREIGN KEY (fk_permissao) REFERENCEs permissao(id)
);

CREATE TABLE categoria_componente (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE tipo_servico (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE status_os (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE grau_urgencia (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE tipo_movimentacao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE tipo_transacao_financeira (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE status_transacao_financeira (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE configuracao_sistema (
    id INT AUTO_INCREMENT PRIMARY KEY,
    chave VARCHAR(100) NOT NULL UNIQUE,
    valor TEXT NOT NULL,
    descricao VARCHAR(255) NULL,
    data_atualizacao DATETIME DEFAULT CURRENT_TIMEsTAMP
);

CREATE TABLE usuario (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome_completo VARCHAR(150) NOT NULL,
    cpf VARCHAR(14) NOT NULL UNIQUE,
    telefone VARCHAR(15) NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    fk_cargo INT NOT NULL,
    senha_hash VARCHAR(255) NOT NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMEsTAMP,
    ativo BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (fk_cargo) REFERENCEs cargo(id)
);

CREATE TABLE usuario_perfil (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_usuario INT NOT NULL,
    fk_perfil INT NOT NULL,
    FOREIGN KEY (fk_usuario) REFERENCEs usuario(id),
    FOREIGN KEY (fk_perfil) REFERENCEs perfil(id)
);

CREATE TABLE cliente (
    id INT AUTO_INCREMENT PRIMARY KEY,
    razao_social VARCHAR(150) NOT NULL,
    nome_fantasia VARCHAR(150) NULL,
    cnpj_cpf VARCHAR(18) NOT NULL UNIQUE,
    telefone VARCHAR(15) NULL,
    email VARCHAR(100) NULL,
    endereco VARCHAR(255) NULL,
    cidade VARCHAR(100) NULL,
    estado VARCHAR(2) NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMEsTAMP,
    ativo BOOLEAN DEFAULT TRUE
);

CREATE TABLE fornecedor (
    id INT AUTO_INCREMENT PRIMARY KEY,
    razao_social VARCHAR(150) NOT NULL,
    nome_fantasia VARCHAR(150) NULL,
    cnpj_cpf VARCHAR(18) NOT NULL UNIQUE,
    telefone VARCHAR(15) NULL,
    email VARCHAR(100) NULL,
    endereco VARCHAR(255) NULL,
    cidade VARCHAR(100) NULL,
    estado VARCHAR(2) NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMEsTAMP,
    ativo BOOLEAN DEFAULT TRUE
);

CREATE TABLE equipamento (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_cliente INT NOT NULL,
    modelo VARCHAR(100) NOT NULL,
    marca_fabricante VARCHAR(100) NOT NULL,
    capacidade_maxima_kg DECIMAL(10,3) NOT NULL,
    divisao_escala_g DECIMAL(10,3) NOT NULL,
    numero_serie VARCHAR(50) NOT NULL UNIQUE,
    setor_localizacao VARCHAR(100) NOT NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMEsTAMP,
    ativo BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (fk_cliente) REFERENCEs cliente(id)
);

CREATE TABLE componente (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(150) NOT NULL,
    codigo_item VARCHAR(50) NOT NULL UNIQUE,
    fk_categoria INT NOT NULL,
    quantidade_estoque INT NOT NULL DEFAULT 0,
    preco_unitario DECIMAL(18,2) NOT NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMEsTAMP,
    ativo BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (fk_categoria) REFERENCEs categoria_componente(id)
);

CREATE TABLE ordem_servico (
    id INT AUTO_INCREMENT PRIMARY KEY,
    numero_os VARCHAR(50) NOT NULL UNIQUE,
    fk_cliente INT NOT NULL,
    fk_equipamento INT NOT NULL,
    fk_tipo_servico INT NOT NULL,
    fk_tecnico INT NOT NULL,
    descricao_problema TEXT NOT NULL,
    fk_status INT NOT NULL,
    data_abertura DATETIME NOT NULL DEFAULT CURRENT_TIMEsTAMP,
    data_conclusao DATETIME NULL,
    FOREIGN KEY (fk_cliente) REFERENCEs cliente(id),
    FOREIGN KEY (fk_equipamento) REFERENCEs equipamento(id),
    FOREIGN KEY (fk_tipo_servico) REFERENCEs tipo_servico(id),
    FOREIGN KEY (fk_tecnico) REFERENCEs usuario(id),
    FOREIGN KEY (fk_status) REFERENCEs status_os(id)
);

CREATE TABLE ordem_servico_peca (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_ordem_servico_principal INT NULL,
    fk_componente INT NOT NULL,
    quantidade INT NOT NULL,
    fk_urgencia INT NOT NULL,
    observacoes TEXT NOT NULL,
    fk_status INT NOT NULL,
    data_solicitacao DATETIME NOT NULL DEFAULT CURRENT_TIMEsTAMP,
    data_resolucao DATETIME NULL,
    FOREIGN KEY (fk_ordem_servico_principal) REFERENCEs ordem_servico(id),
    FOREIGN KEY (fk_componente) REFERENCEs componente(id),
    FOREIGN KEY (fk_urgencia) REFERENCEs grau_urgencia(id),
    FOREIGN KEY (fk_status) REFERENCEs status_os(id)
);

CREATE TABLE certificado_calibracao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    numero_certificado VARCHAR(50) NOT NULL UNIQUE,
    fk_ordem_servico INT NULL,
    fk_equipamento INT NOT NULL,
    data_calibracao DATE NOT NULL,
    data_proxima_calibracao DATE NOT NULL,
    temperatura_ambiente DECIMAL(5,2) NOT NULL,
    umidade_relativa DECIMAL(5,2) NOT NULL,
    fk_tecnico_responsavel INT NOT NULL,
    data_emissao DATETIME NOT NULL DEFAULT CURRENT_TIMEsTAMP,
    FOREIGN KEY (fk_ordem_servico) REFERENCEs ordem_servico(id),
    FOREIGN KEY (fk_equipamento) REFERENCEs equipamento(id),
    FOREIGN KEY (fk_tecnico_responsavel) REFERENCEs usuario(id)
);

CREATE TABLE movimentacao_estoque (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_componente INT NOT NULL,
    fk_tipo_movimentacao INT NOT NULL,
    motivo_movimentacao VARCHAR(150) NOT NULL,
    quantidade INT NOT NULL,
    custo_unitario DECIMAL(18,2) NOT NULL,
    fk_fornecedor INT NULL,
    fk_ordem_servico INT NULL,
    fk_usuario_responsavel INT NOT NULL,
    data_movimentacao DATETIME NOT NULL DEFAULT CURRENT_TIMEsTAMP,
    FOREIGN KEY (fk_componente) REFERENCEs componente(id),
    FOREIGN KEY (fk_tipo_movimentacao) REFERENCEs tipo_movimentacao(id),
    FOREIGN KEY (fk_fornecedor) REFERENCEs fornecedor(id),
    FOREIGN KEY (fk_ordem_servico) REFERENCEs ordem_servico(id),
    FOREIGN KEY (fk_usuario_responsavel) REFERENCEs usuario(id)
);

CREATE TABLE transacao_financeira (
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
    data_cadastro DATETIME NOT NULL DEFAULT CURRENT_TIMEsTAMP,
    FOREIGN KEY (fk_tipo_transacao) REFERENCEs tipo_transacao_financeira(id),
    FOREIGN KEY (fk_status) REFERENCEs status_transacao_financeira(id),
    FOREIGN KEY (fk_cliente) REFERENCEs cliente(id),
    FOREIGN KEY (fk_fornecedor) REFERENCEs fornecedor(id),
    FOREIGN KEY (fk_ordem_servico) REFERENCEs ordem_servico(id)
);

CREATE TABLE auditoria_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tabela_afetada VARCHAR(100) NOT NULL,
    registro_id INT NOT NULL,
    acao VARCHAR(20) NOT NULL,
    dados_antigos TEXT NULL,
    dados_novos TEXT NULL,
    fk_usuario INT NULL,
    data_hora DATETIME NOT NULL DEFAULT CURRENT_TIMEsTAMP,
    FOREIGN KEY (fk_usuario) REFERENCEs usuario(id)
);

-- ==========================================
-- DADOs INICIAIs (sEED) PARA O sIsTEMA
-- ==========================================

INsERT INTO cargo (nome, descricao) VALUEs 
('T�cnico de Calibra��o', 'T�cnico respons�vel pelas aferi��es'),
('Engenheiro', 'Engenheiro metrologista'),
('Administrativo', 'Equipe de escrit�rio e atendimento'),
('Gerente', 'Gerente da unidade');

INsERT INTO perfil (nome, descricao) VALUEs 
('Admin', 'Acesso total ao sistema'),
('Tecnico', 'Acesso �s Os e calibra��es'),
('Atendimento', 'Acesso a clientes e balan�as');

INsERT INTO tipo_servico (nome) VALUEs 
('Calibra��o de Balan�a Anal�tica'),
('Manuten��o Preventiva'),
('Manuten��o Corretiva'),
('Aferi��o de Padr�es');

INsERT INTO status_os (nome) VALUEs 
('Pendente'),
('Em Andamento'),
('Aguardando Pe�a'),
('Conclu�do'),
('Cancelado');

INsERT INTO grau_urgencia (nome) VALUEs 
('Baixa'),
('M�dia'),
('Alta'),
('Cr�tica');

INsERT INTO tipo_movimentacao (nome) VALUEs 
('Entrada (Compra)'),
('sa�da (Os)'),
('Ajuste de Estoque (+/-)');

INsERT INTO categoria_componente (nome) VALUEs 
('sensores'),
('Placas de Circuito'),
('Cabos'),
('P�s de Borracha'),
('Displays');

INsERT INTO tipo_transacao_financeira (nome) VALUEs 
('Receita'),
('Despesa');

INsERT INTO status_transacao_financeira (nome) VALUEs 
('Pendente'),
('Pago'),
('Atrasado'),
('Cancelado');



