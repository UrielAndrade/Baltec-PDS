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
    FOREIGN KEY (fk_perfil) REFERENCES perfil(id),
    FOREIGN KEY (fk_permissao) REFERENCES permissao(id)
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
    data_atualizacao DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE usuario (
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
);

CREATE TABLE usuario_perfil (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_usuario INT NOT NULL,
    fk_perfil INT NOT NULL,
    FOREIGN KEY (fk_usuario) REFERENCES usuario(id),
    FOREIGN KEY (fk_perfil) REFERENCES perfil(id)
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
    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
    ativo BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (fk_cliente) REFERENCES cliente(id)
);

CREATE TABLE componente (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(150) NOT NULL,
    codigo_item VARCHAR(50) NOT NULL UNIQUE,
    fk_categoria INT NOT NULL,
    quantidade_estoque INT NOT NULL DEFAULT 0,
    preco_unitario DECIMAL(18,2) NOT NULL,
    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
    ativo BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (fk_categoria) REFERENCES categoria_componente(id)
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
    data_abertura DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_conclusao DATETIME NULL,
    FOREIGN KEY (fk_cliente) REFERENCES cliente(id),
    FOREIGN KEY (fk_equipamento) REFERENCES equipamento(id),
    FOREIGN KEY (fk_tipo_servico) REFERENCES tipo_servico(id),
    FOREIGN KEY (fk_tecnico) REFERENCES usuario(id),
    FOREIGN KEY (fk_status) REFERENCES status_os(id)
);

CREATE TABLE ordem_servico_peca (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fk_ordem_servico_principal INT NULL,
    fk_componente INT NOT NULL,
    quantidade INT NOT NULL,
    fk_urgencia INT NOT NULL,
    observacoes TEXT NOT NULL,
    fk_status INT NOT NULL,
    data_solicitacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_resolucao DATETIME NULL,
    FOREIGN KEY (fk_ordem_servico_principal) REFERENCES ordem_servico(id),
    FOREIGN KEY (fk_componente) REFERENCES componente(id),
    FOREIGN KEY (fk_urgencia) REFERENCES grau_urgencia(id),
    FOREIGN KEY (fk_status) REFERENCES status_os(id)
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
    data_emissao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (fk_ordem_servico) REFERENCES ordem_servico(id),
    FOREIGN KEY (fk_equipamento) REFERENCES equipamento(id),
    FOREIGN KEY (fk_tecnico_responsavel) REFERENCES usuario(id)
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
    data_movimentacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (fk_componente) REFERENCES componente(id),
    FOREIGN KEY (fk_tipo_movimentacao) REFERENCES tipo_movimentacao(id),
    FOREIGN KEY (fk_fornecedor) REFERENCES fornecedor(id),
    FOREIGN KEY (fk_ordem_servico) REFERENCES ordem_servico(id),
    FOREIGN KEY (fk_usuario_responsavel) REFERENCES usuario(id)
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
    data_cadastro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (fk_tipo_transacao) REFERENCES tipo_transacao_financeira(id),
    FOREIGN KEY (fk_status) REFERENCES status_transacao_financeira(id),
    FOREIGN KEY (fk_cliente) REFERENCES cliente(id),
    FOREIGN KEY (fk_fornecedor) REFERENCES fornecedor(id),
    FOREIGN KEY (fk_ordem_servico) REFERENCES ordem_servico(id)
);

CREATE TABLE auditoria_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tabela_afetada VARCHAR(100) NOT NULL,
    registro_id INT NOT NULL,
    acao VARCHAR(20) NOT NULL,
    dados_antigos TEXT NULL,
    dados_novos TEXT NULL,
    fk_usuario INT NULL,
    data_hora DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (fk_usuario) REFERENCES usuario(id)
);
