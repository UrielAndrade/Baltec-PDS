# Como Rodar o Banco de Dados Localmente (Tutorial para a Equipe)

Este guia rápido foi criado para que **todos os desenvolvedores** (estudantes/equipe) consigam rodar o banco de dados do Baltec-PDS na própria máquina sem dificuldades.

---

## 🛠️ Opção 1: Usando o XAMPP (Mais fácil para iniciantes)

O XAMPP é um pacote que já vem com o MySQL e uma interface visual chamada phpMyAdmin.

1. **Baixe e instale o XAMPP** (https://www.apachefriends.org/pt_br/index.html).
2. Abra o **XAMPP Control Panel**.
3. Clique em `Start` ao lado de **MySQL** (e também do Apache, se for usar o phpMyAdmin).
4. No seu navegador, acesse: [http://localhost/phpmyadmin](http://localhost/phpmyadmin).
5. No menu superior, clique na aba **SQL**.
6. Copie todo o conteúdo do arquivo `banco_de_dados.sql` (que está na pasta principal do projeto) e cole na caixa de texto do SQL.
7. Clique no botão **Executar** no canto inferior direito.
   - *Pronto! O banco `baltec` e todas as tabelas foram criados e os dados iniciais foram inseridos.*

---

## 🛠️ Opção 2: Usando o MySQL Workbench ou DBeaver

Se você já instalou o MySQL puramente (via MySQL Installer) e usa o Workbench ou o DBeaver:

1. Abra o seu gerenciador de banco de dados (MySQL Workbench, DBeaver, HeidiSQL).
2. Crie uma nova conexão (geralmente em `localhost` na porta `3306` com usuário `root`).
3. Abra a conexão.
4. Vá em **File > Open SQL Script...** (ou apenas crie uma nova aba de Query).
5. Selecione/Copie o conteúdo do arquivo `banco_de_dados.sql`.
6. Clique no ícone de **Raio** (Executar/Execute Script).
   - *Pronto! O banco foi criado e as tabelas populadas.*

---

## ⚙️ Conectando o Projeto Blazor ao seu Banco

No nosso projeto, nós estamos utilizando o arquivo `appsettings.json` para dizer ao C# como se conectar ao MySQL.

Se você está rodando o XAMPP ou o MySQL padrão, normalmente o usuário é `root` e a senha é vazia (`""`) ou a senha que você escolheu na instalação.

Abra o arquivo `appsettings.json` do projeto e verifique se a `DefaultConnection` corresponde à sua máquina:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=baltec;User=root;Password=suasenha;"
}
```

* **Dica Importante:** Se você usa o XAMPP e ele não tem senha, deixe assim: `Password=;`.
* Depois de ajustar a senha, rode o projeto (`dotnet run` ou aperte *Play* no Visual Studio) e acesse a aba de Cadastro para ver se o sistema carregou os Cargos corretamente do banco de dados!

---

**Nota para a equipe:** Se futuramente vocês hospedarem o banco na nuvem (AWS, Aiven, Hostinger), não precisarão fazer nada disso localmente. Bastará trocar a `DefaultConnection` para o endereço do servidor online!
