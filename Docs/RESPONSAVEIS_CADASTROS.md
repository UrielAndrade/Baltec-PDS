# Responsáveis pelas Funcionalidades de Cadastro — Baltec PDS

Este documento mapeia e detalha **quais desenvolvedores são responsáveis por cada funcionalidade de cadastro** no sistema **Baltec PDS**, tendo como referência o [PLANO_IMPLEMENTACAO.md](file:///c:/Users/2024102020056/Documents/GitHub/Baltec-PDS/Docs/PLANO_IMPLEMENTACAO.md).

---

## 1. Visão Geral Consolidada

O sistema Baltec PDS divide as funcionalidades de cadastro em duas categorias:
1. **Cadastros Base / Cadastrais (Cadastros Mestres):** Usuários/Colaboradores, Clientes, Balanças/Equipamentos e Componentes/Peças.
2. **Cadastros Operacionais / Transacionais:** Ordens de Serviço, Requisições de Peças, Certificados de Calibração e Transações Financeiras.

### Matriz de Responsabilidades

| Pessoa / Desenvolvedor | Funcionalidade(s) de Cadastro | Tipo de Cadastro | Arquivo(s) de Modelo | Arquivo(s) DAO & Métodos de Cadastro | Interface / Tela Blazor |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Alisson** | Cadastro de Usuários / Colaboradores | Cadastro Base | `models/Usuario.cs`<br/>`models/Cargo.cs` | `DAO/UsuarioDAO.cs`<br/>• `CadastrarAsync()`<br/>• `EmailExisteAsync()`<br/>• `CpfExisteAsync()` | `Components/Pages/Cadastro.razor` |
| **Kamilly** | • Cadastro de Clientes<br/>• Cadastro de Balanças / Equipamentos | Cadastro Base | `models/Cliente.cs`<br/>`models/Equipamento.cs` | `DAO/ClienteDAO.cs`<br/>• `InserirAsync(Cliente)`<br/>`DAO/EquipamentoDAO.cs`<br/>• `InserirAsync(Equipamento)` | `Components/Pages/Balancas.razor`<br/>(Modais/Formulários de criação) |
| **Letícia** | • Cadastro de Componentes / Peças<br/>• Cadastro de Categorias<br/>• Registro de Entradas no Estoque | Cadastro Base e Estoque | `models/Componente.cs`<br/>`models/CategoriaComponente.cs`<br/>`models/MovimentacaoEstoque.cs` | `DAO/ComponenteDAO.cs`<br/>• `InserirAsync(Componente)`<br/>`DAO/MovimentacaoEstoqueDAO.cs`<br/>• `RegistrarMovimentacaoAsync()` | `Components/Pages/Componentes.razor`<br/>(Modal de nova peça e entrada de estoque) |
| **Uriel Luiz de Andrade** | Cadastro / Abertura de Ordem de Serviço (OS) | Cadastro Operacional | `models/OrdemServico.cs`<br/>`models/TipoServico.cs`<br/>`models/StatusOS.cs` | `DAO/OrdemServicoDAO.cs`<br/>• `InserirAsync(OrdemServico)`<br/>• `GerarProximoNumeroOSAsync()` | `Components/Pages/OrdensServico.razor`<br/>(Modal/Form "+ Abrir Nova Ordem de Serviço") |
| **Victor Henrique** | Cadastro / Solicitação de Requisição de Peças (OS Peça) | Cadastro Operacional | `models/OrdemServicoPeca.cs`<br/>`models/GrauUrgencia.cs` | `DAO/OrdemServicoPecaDAO.cs`<br/>• `SolicitarPecaAsync(OrdemServicoPeca)` | `Components/Pages/OSPecas.razor`<br/>(Modal/Botão "Nova Requisição de Peça") |
| **Samuel Borges** | Cadastro e Emissão de Certificados de Calibração Metrológica | Cadastro Operacional | `models/CertificadoCalibracao.cs` | `DAO/CertificadoCalibracaoDAO.cs`<br/>• `InserirAsync(CertificadoCalibracao)`<br/>• `GerarProximoNumeroCertificadoAsync()` | `Components/Pages/Certificados.razor`<br/>(Formulário de Emissão de Certificado) |
| **Arthur Braga** | Cadastro / Lançamento de Transações Financeiras (Receitas/Despesas) | Cadastro Financeiro | `models/TransacaoFinanceira.cs` | `DAO/FinanceiroDAO.cs`<br/>• `InserirAsync(TransacaoFinanceira)` | `Components/Pages/Financeiro.razor`<br/>(Modal de Lançamento de Receita/Despesa) |

---

## 2. Detalhamento por Desenvolvedor

### 1. Alisson — Cadastro de Usuários / Colaboradores
* **O que cadastra:** Novos colaboradores com acesso ao sistema Baltec (técnicos, administradores, atendentes).
* **Campos coletados:** Nome completo, CPF, telefone, e-mail, cargo (`FkCargo`) e senha com hash.
* **Componentes e Arquivos:**
  - Tela: `Components/Pages/Cadastro.razor`
  - Modelo: `models/Usuario.cs` e `models/Cargo.cs`
  - DAO: `DAO/UsuarioDAO.cs`
* **Regras de Validação no Cadastro:**
  - Verificar unicidade do E-mail via `EmailExisteAsync(email)`.
  - Verificar unicidade do CPF via `CpfExisteAsync(cpf)`.
  - Criptografia/hash seguro da senha antes de persistir no banco.
  - Carregar dropdown de cargos cadastrados (`ListarCargosAsync()`).

---

### 2. Kamilly — Cadastro de Clientes e Balanças/Equipamentos
* **O que cadastra:**
  1. **Clientes:** Empresas, indústrias, comércios ou produtores que contratam os serviços da Baltec.
  2. **Balanças / Equipamentos:** Equipamentos de pesagem que passam por manutenção ou calibração.
* **Campos coletados:**
  - **Cliente:** Razão Social, Nome Fantasia, CNPJ/CPF, Telefone, E-mail, Endereço, Cidade, Estado.
  - **Balança/Equipamento:** Cliente proprietário (`FkCliente`), Modelo, Marca/Fabricante, Capacidade Máxima (kg), Divisão de Escala (g), Número de Série e Setor/Localização.
* **Componentes e Arquivos:**
  - Tela: `Components/Pages/Balancas.razor`
  - Modelos: `models/Cliente.cs` e `models/Equipamento.cs`
  - DAOs: `DAO/ClienteDAO.cs` e `DAO/EquipamentoDAO.cs`
* **Regras de Validação no Cadastro:**
  - Validação de formato e duplicidade de CNPJ/CPF.
  - Validação de duplicidade do Número de Série da balança (`BuscarPorNumeroSerieAsync`).
  - Vinculação obrigatória da balança a um cliente previamente cadastrado.

---

### 3. Letícia — Cadastro de Peças/Componentes e Registro de Estoque
* **O que cadastra:**
  1. **Componentes/Peças:** Catálogo de peças de reposição e insumos para manutenção de balanças.
  2. **Entrada de Estoque:** Registro inicial ou reposição de componentes no almoxarifado.
* **Campos coletados:**
  - **Componente:** Código do Item, Nome da Peça, Categoria (`FkCategoria`), Quantidade Inicial em Estoque, Preço Unitário.
  - **Movimentação:** Componente (`FkComponente`), Tipo de Movimentação (Entrada/Ajuste), Quantidade, Custo Unitário, Motivo, Usuário Responsável.
* **Componentes e Arquivos:**
  - Tela: `Components/Pages/Componentes.razor` (ou modais auxiliares)
  - Modelos: `models/Componente.cs`, `models/CategoriaComponente.cs` e `models/MovimentacaoEstoque.cs`
  - DAOs: `DAO/ComponenteDAO.cs` e `DAO/MovimentacaoEstoqueDAO.cs`
* **Regras de Validação no Cadastro:**
  - Unicidade do Código do Item (`BuscarPorCodigoAsync`).
  - Validação de valores monetários e quantidades positivas.
  - Ao registrar entrada, atualizar de forma transacional o saldo da peça (`AtualizarQuantidadeEstoqueAsync`).

---

### 4. Uriel Luiz de Andrade — Cadastro / Abertura de Ordem de Serviço (OS)
* **O que cadastra:** Abertura formal de OS para atendimento técnico de manutenção ou calibração.
* **Campos coletados:** Cliente (`FkCliente`), Equipamento do Cliente (`FkEquipamento`), Tipo de Serviço (`FkTipoServico`), Técnico Responsável (`FkTecnico`), Descrição detalhada do problema ou serviço requisitado.
* **Componentes e Arquivos:**
  - Tela: `Components/Pages/OrdensServico.razor`
  - Modelos: `models/OrdemServico.cs`, `models/TipoServico.cs` e `models/StatusOS.cs`
  - DAO: `DAO/OrdemServicoDAO.cs`
* **Regras de Validação no Cadastro:**
  - Geração automática e sequencial do número da OS (ex: `OS-2026-0001`) via `GerarProximoNumeroOSAsync()`.
  - Dropdown em cascata: selecionar Cliente carrega apenas os equipamentos vinculados àquele cliente.
  - Inicialização automática com status "Pendente" ou "Em Aberto" e registro de `DataAbertura`.

---

### 5. Victor Henrique — Cadastro de Requisição de Peças (OS de Peça)
* **O que cadastra:** Solicitação formal de peças do estoque vinculadas a uma OS em andamento.
* **Campos coletados:** OS Principal (`FkOrdemServicoPrincipal`), Peça solicitada (`FkComponente`), Quantidade requisitada, Grau de Urgência (`FkUrgencia`: Baixa, Média, Alta, Crítica) e Observações/Justificativa técnica.
* **Componentes e Arquivos:**
  - Tela: `Components/Pages/OSPecas.razor`
  - Modelos: `models/OrdemServicoPeca.cs` e `models/GrauUrgencia.cs`
  - DAO: `DAO/OrdemServicoPecaDAO.cs`
* **Regras de Validação no Cadastro:**
  - Validar se a OS de referência está ativa e não cancelada/finalizada.
  - Validar se a quantidade solicitada é maior que zero.
  - Gravação do registro com status inicial de "Pendente de Aprovação".

---

### 6. Samuel Borges — Cadastro / Emissão de Certificados de Calibração
* **O que cadastra:** Certificado metrológico atestando a conformidade da balança com normas técnicas do INMETRO.
* **Campos coletados:** Balança/Equipamento aferido (`FkEquipamento`), OS vinculada (`FkOrdemServico`), Técnico responsável (`FkTecnicoResponsavel`), Condições ambientais (Temperatura em °C e Umidade Relativa em %), Data de Calibração.
* **Componentes e Arquivos:**
  - Tela: `Components/Pages/Certificados.razor`
  - Modelo: `models/CertificadoCalibracao.cs`
  - DAO: `DAO/CertificadoCalibracaoDAO.cs`
* **Regras de Validação no Cadastro:**
  - Geração automática do número do certificado (ex: `CERT-2026-0001`) via `GerarProximoNumeroCertificadoAsync()`.
  - Cálculo automático da `DataProximaCalibracao` (+1 ano a partir da data de calibração).
  - Verificação de preenchimento obrigatório das condições ambientais e do padrão metrológico.

---

### 7. Arthur Braga — Cadastro / Lançamento de Transações Financeiras
* **O que cadastra:** Contas a receber (faturamento de serviços prestados) e contas a pagar (despesas operacionais, compras de peças).
* **Campos coletados:** Tipo de transação (Receita ou Despesa), Descrição, Valor (R$), Data de Vencimento, Vínculo opcional com Cliente (`FkCliente`), Fornecedor (`FkFornecedor`) ou OS (`FkOrdemServico`).
* **Componentes e Arquivos:**
  - Tela: `Components/Pages/Financeiro.razor`
  - Modelo: `models/TransacaoFinanceira.cs`
  - DAO: `DAO/FinanceiroDAO.cs`
* **Regras de Validação no Cadastro:**
  - Validação de valor positivo (`Valor > 0`).
  - Definição do status inicial como "Pendente" com data de vencimento válida.
  - Atualização dos totais e dashboards ao registrar novas transações.

