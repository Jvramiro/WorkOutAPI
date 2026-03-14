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

## Endpoints

### Public Endpoints
No authentication required to access these endpoints:
*   `POST /api/Login` - Authenticate a user and receive JWT tokens.
*   `POST /api/Register` - Register a new user in the system.
*   `GET /api/Exercises` - Retrieve a paginated list of exercises.
*   `GET /api/Exercises/{id}` - Retrieve a specific exercise by ID.

### Private Endpoints - Admin Role
These endpoints require a valid JWT token in the `Authorization` header (`Bearer <token>`):
These endpoints are only accessible by users with the Admin role.
*   `GET /api/Users` - Retrieve a paginated list of users.
*   `GET /api/Users/{id}` - Retrieve a specific user by ID.
*   `POST /api/Exercises` - Create a new exercise.
*   `PUT /api/Exercises/{id}` - Update an existing exercise.
*   `DELETE /api/Exercises/{id}` - Remove an exercise from the system.
*   `GET /api/CheckIn/{userId}` - Retrieve check-ins for a specific user.

### Private Endpoints - Any Role
These endpoints require a valid JWT token in the `Authorization` header (`Bearer <token>`):
*   `POST /api/Login/refresh` - Refresh an expired JWT token.
*   `PUT /api/Users/{id}` - Update a user's details or schedule (User or Admin).
*   `DELETE /api/Users/{id}` - Remove a user from the system (User or Admin).
*   `GET /api/CheckIn/self` - Retrieve check-ins for the currently authenticated user.
*   `POST /api/CheckIn` - Create a new check-in for the currently authenticated user.
*   `DELETE /api/CheckIn/{id}` - Remove a check-in (User or Admin).

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

## Seeders

The database seeds initial data if missing on application startup:
*   **Admin User**: An initial admin is created with the email `admin@mail.com` and password `12345678`.
*   **Exercises**: A predefined base list of exercises is seeded from `base_exercises.json`.

---

# Security

Secrets are not stored in the repository.

Configuration is handled using:

- **User Secrets** (local development)
- **Environment Variables** (Docker)

---

# License

This project is available under the MIT License.