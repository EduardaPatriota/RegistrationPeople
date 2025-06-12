# Registration of people

> Sistema para cadastro de pessoas com autenticação e boas práticas em arquitetura de software, desenvolvido com ASP.NET Core 8 no backend e React no frontend.

## 📌 Descrição

O **RegistrationPeople** é uma aplicação completa para gerenciamento de cadastro de pessoas, composta por uma API RESTful em ASP.NET Core 8 e um front-end moderno em React. Possui autenticação via JWT, versionamento da API, documentação interativa com Swagger e arquitetura organizada em camadas.

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- [Node.js](https://nodejs.org/) 16 ou superior  
- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- Banco de dados InMemory (configurado no projeto)  
- [Git](https://git-scm.com/)

## 🚀 Funcionalidades

- ✅ Autenticação e autorização com JWT  
- 📝 CRUD completo de pessoas  
- 📄 Versionamento de API  
- 📚 Documentação interativa com Swagger  
- 📦 Separação clara entre camadas (Domain, Application, Infrastructure, API)  
- 💻 Front-end desenvolvido em React  

## ⚙️ Tecnologias Utilizadas

- **Backend:** ASP.NET Core 8  
- **Banco de Dados:** SQLite / InMemory  
- **Autenticação:** JWT  
- **Documentação:** Swagger  
- **Frontend:** React  
- **Arquitetura:** Clean Architecture (Domain, Application, Infrastructure, API)  

## 🛠️ Como Executar

### Backend (.NET 8)

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/PeopleRegistration.git
cd PeopleRegistration
```

2. Navegue até a pasta da API:
```bash
cd RegistrationPeopleApi
```

3. Restaure os pacotes e execute as migrações:
```bash
dotnet restore
dotnet ef database update
```

4. Execute a API:
```bash
dotnet run
```
A API estará disponível em `https://localhost:7247`

### Frontend (React)

1. Navegue até a pasta do frontend:
```bash
cd RegistrationPeopleApp
```

2. Instale as dependências:
```bash
npm install
```

3. Execute o projeto:
```bash
npm run dev
```
O frontend estará disponível em `http://localhost:8080`
