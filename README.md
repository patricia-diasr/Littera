# 📚 Littera

O **Littera** é um sistema desenvolvido para facilitar a administração de leituras pessoais, oferecendo ao usuário uma plataforma intuitiva e funcional para organizar sua experiência com livros. Criado como parte da disciplina de **Projeto de Bloco em Desenvolvimento Back-end**, o sistema busca unir praticidade e controle em uma única aplicação.

---

## 🧩 Funcionalidades

- 📖 Cadastrar livros com capa e informações detalhadas  
- 🔄 Gerenciar o status da leitura: **"Quero ler"**, **"Lendo"**, **"Lido"**  
- 🗓️ Registrar datas de início e término da leitura  
- 📝 Fazer anotações e avaliações com até 5 estrelas  
- 🧱 Visualizar livros em formato de vitrine  
- 🔍 Buscar títulos rapidamente  
- 📊 Acompanhar histórico de leitura e estatísticas (total de livros lidos, progresso anual)

---

## 🛠️ Tecnologias utilizadas

- **.NET**: Framework principal da aplicação, com suporte ao desenvolvimento web moderno e performático.  
- **Razor Pages**: Criação de páginas dinâmicas unindo lógica de servidor com HTML.  
- **Entity Framework**: ORM para mapear objetos C# ao banco de dados com facilidade.  
- **SQLite**: Banco de dados leve e eficiente para armazenar os dados dos usuários e livros.  
- **HTML5 & CSS3**: Estruturação e estilização da interface, garantindo uma navegação intuitiva.

---

## 💻 Como rodar o projeto localmente

> ⚠️ Pré-requisitos: [.NET SDK](https://dotnet.microsoft.com/en-us/download), IDE compatível (Visual Studio ou VS Code)

```bash
# Clone o repositório
git clone https://github.com/patricia-diasr/Littera.git

# Acesse a pasta raiz do projeto
cd Littera

# Restaure as dependências
dotnet restore

# Acesse a subpasta onde está o projeto principal
cd Littera

# Rode a aplicação
dotnet run
