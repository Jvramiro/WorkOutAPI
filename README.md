# WorkOutAPI

WorkOutAPI is a RESTful API built with **ASP.NET Core** for managing workouts, exercises, users, and gym check-ins.

This project demonstrates backend best practices such as:

- JWT Authentication
- Repository Pattern
- Environment-based configuration
- Docker support
- Swagger API documentation
- SQLite with Entity Framework Core

Repository:  
https://github.com/Jvramiro/WorkOutAPI

---

# Features

- User registration
- JWT authentication
- Refresh token endpoint
- Exercise management
- Workout check-ins
- Protected endpoints
- Swagger interactive documentation
- Automatic database migration and seeding
- Docker support

---

# Tech Stack

- ASP.NET Core
- Entity Framework Core
- SQLite
- JWT Authentication
- Docker
- Swagger / OpenAPI

---

# Project Structure

```
WorkOutAPI
│
├── src
│   └── WorkOutAPI
│       ├── Configurations
│       ├── Controllers
│       ├── Data
│       ├── DTO
│       ├── Enums
│       ├── Migrations
│       ├── Models
│       ├── Repositories
│       ├── Services
│       ├── Program.cs
│       ├── WorkOutAPI.csproj
│       ├── Dockerfile
│       └── appsettings.json
│
├── docker-compose.yml
├── .env
├── .env.example
├── appsettings.example.json
├── WorkOutAPI.sln
└── README.md
```

The project follows the **Repository Pattern** to separate business logic from database access.

---

# API Documentation

After running the project, Swagger will be available at:

```
http://localhost:8080/swagger
```

Swagger provides an interactive UI to test all endpoints.

Main endpoint groups:

- Users
- Login
- Register
- Exercises
- CheckIns

---

# Authentication

Authentication is implemented using **JSON Web Tokens (JWT)**.

Protected endpoints require:

```
Authorization: Bearer <token>
```

Tokens are obtained through the login endpoint.

---

# Running the Project Locally (.NET)

## 1 Clone the repository

```
git clone https://github.com/Jvramiro/WorkOutAPI.git
cd WorkOutAPI
```

---

## 2 Configure User Secrets

The project uses **.NET User Secrets** to store the JWT signing key during development.

Initialize secrets:

```
dotnet user-secrets init --project src/WorkOutAPI
```

Set the JWT key:

```
dotnet user-secrets set "Security:JwtKey" "your-secret-key" --project src/WorkOutAPI
```

---

## 3 Run the API

```
dotnet run --project src/WorkOutAPI
```

Swagger will be available at:

```
http://localhost:8080/swagger
```

---

# Running with Docker

Docker uses **environment variables instead of User Secrets**.

---

## 1 Create the `.env` file

Copy the example:

```
cp .env.example .env
```

Edit `.env` and configure the JWT key:

```
Security__JwtKey=your-secret-key
ASPNETCORE_ENVIRONMENT=Development
```

---

## 2 Run with Docker

```
docker compose up --build
```

Swagger will be available at:

```
http://localhost:8080/swagger
```

---

# Database

The application uses **SQLite with Entity Framework Core**.

On startup the API automatically:

- Applies database migrations
- Seeds the database with initial data

Database file:

```
LocalDB.db
```

---

# Security

Secrets are not stored in the repository.

Configuration is handled using:

- **User Secrets** (local development)
- **Environment Variables** (Docker)

---

# License

This project is available under the MIT License.