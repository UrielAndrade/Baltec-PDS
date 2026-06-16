# Baltec PDS — Sistema de Gestão Interna

## Sobre a Empresa

A Baltec é uma empresa estabelecida em Ji-Paraná, Rondônia, especializada em **metrologia e calibração de instrumentos de pesagem**. Fundada com o objetivo de oferecer serviços técnicos de alta precisão para indústrias, comércios e órgãos públicos da região, a empresa atua no fornecimento de calibração, manutenção e certificação de balanças e equipamentos de medição em conformidade com as normas do INMETRO.

Entre os principais serviços oferecidos pela Baltec estão:

- Calibração e aferição de balanças comerciais, industriais e rodoviárias
- Emissão de certificados de calibração rastreáveis
- Manutenção corretiva e preventiva de equipamentos de pesagem
- Cadastro e gerenciamento de componentes e peças
- Atendimento técnico especializado para clientes da região de Ji-Paraná e municípios vizinhos

---

## Sobre o Sistema

O **Baltec PDS** (Painel de Desenvolvimento de Software) é um sistema de gestão interna desenvolvido para digitalizar e centralizar os processos operacionais da empresa. O sistema substitui controles manuais em papel e planilhas, oferecendo uma interface moderna, segura e de fácil uso para os colaboradores.

### Funcionalidades previstas

- Autenticacao e controle de acesso por usuario
- Cadastro de balanças e equipamentos atendidos
- Cadastro de componentes e pecas utilizados nos servicos
- Geracao e acompanhamento de ordens de servico
- Emissao de certificados de calibracao
- Geracao de ordens de servico de pecas
- Modulo financeiro para acompanhamento de faturamento e despesas
- Painel de indicadores (dashboard) com graficos e metricas operacionais

---

## Tecnologias Utilizadas

| Tecnologia | Versao | Finalidade |
|---|---|---|
| .NET 8 | 8.0 | Backend e servidor |
| Blazor Web App | .NET 8 | Framework de interface |
| C# | 12 | Linguagem principal |
| HTML / CSS | — | Estrutura e estilo das paginas |
| Chart.js | 4.4.0 | Graficos no painel |
| Google Fonts (Inter) | — | Tipografia |

A arquitetura escolhida e o **Blazor Server com render mode interativo**, permitindo que toda a logica de negocio rode no servidor enquanto a interface e renderizada no navegador em tempo real via SignalR. Isso garante desempenho e seguranca sem necessidade de uma API separada na fase inicial do projeto.

---

## Estrutura do Projeto

```
Baltec-PDS/
├── Components/
│   ├── App.razor               # Documento raiz HTML, imports de fontes e scripts
│   ├── Routes.razor            # Configuracao de rotas
│   ├── _Imports.razor          # Namespaces globais do Blazor
│   ├── Layout/
│   │   └── MainLayout.razor    # Layout base das paginas
│   └── Pages/
│       ├── Login.razor         # Tela de autenticacao
│       ├── Login.razor.css     # Estilos isolados do Login
│       ├── Cadastro.razor      # Tela de cadastro de usuario
│       ├── Cadastro.razor.css  # Estilos isolados do Cadastro
│       ├── Home.razor          # Painel principal (dashboard)
│       ├── Home.razor.css      # Estilos isolados do Painel
│       └── Error.razor         # Pagina de erro
├── wwwroot/
│   ├── app.css                 # Estilos globais e variaveis CSS
│   └── imgs/
│       ├── baltec.svg          # Logo da empresa
│       └── eye.svg             # Icone de visibilidade de senha
├── Program.cs                  # Configuracao e inicializacao da aplicacao
├── Baltec.csproj               # Arquivo de projeto .NET
└── appsettings.json            # Configuracoes da aplicacao
```

---

## Telas Implementadas

### Login (`/`)

Tela inicial do sistema. O usuario informa seu e-mail e senha para acessar o painel. Possui alternancia de visibilidade da senha e validacao de campos em tempo real. O link "Cadastre-se" direciona para a tela de cadastro.

### Cadastro (`/cadastro`)

Tela de criacao de conta para novos colaboradores. Coleta nome completo, CPF, telefone, e-mail, cargo e senha. Realiza validacoes de todos os campos antes de prosseguir, exibe um indicador de carregamento durante o processamento e redireciona para o login apos o cadastro bem-sucedido.

### Painel (`/home`)

Dashboard principal do sistema. Exibe indicadores de desempenho (KPIs), graficos de ordens de servico por mes e produtos por categoria, e uma tabela com as ultimas ordens de servico registradas. A barra lateral apresenta todos os modulos do sistema, sendo que apenas o modulo "Inicio" esta ativo nesta versao; os demais exibem uma notificacao informando que estao em desenvolvimento.

---

## Como Executar Localmente

**Requisitos:**

- .NET 8 SDK instalado
- Navegador moderno (Chrome, Edge, Firefox)

**Passos:**

```bash
# Clone o repositorio
git clone https://github.com/UrielAndrade/Baltec-PDS.git

# Entre na pasta do projeto
cd Baltec-PDS

# Execute o servidor de desenvolvimento
dotnet run
```

Apos a inicializacao, acesse `http://localhost:5015` no navegador.

---

## Historico de Desenvolvimento

| Versao | Descricao |
|---|---|
| 0.1.0 | Estrutura inicial do projeto Blazor, primeiros estilos e logo |
| 0.2.0 | Tela de Login com design finalizado, CSS isolado e variaveis de cor |
| 1.0.0 | Implementacao completa: Login funcional, tela de Cadastro, Painel com graficos e tabela de OS |

---

## Equipe

Projeto desenvolvido pelos alunos do curso de Desenvolvimento de Sistemas em parceria com a empresa **Baltec**, sediada em Ji-Paraná — RO.

---

## Licenca

Este projeto e de uso interno e academico. Todos os direitos reservados a Baltec — Ji-Paraná, Rondonia, Brasil.
