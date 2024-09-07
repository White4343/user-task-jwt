# UserTaskJWT
UserTaskJWT is a .NET 8 Web API project for managing user tasks, incorporating JSON Web Tokens (JWT) for authentication and authorization. It demonstrates a Clean Architecture approach with a focus on maintainability and testability.

## Setup Instructions
### Prerequisites
  .NET 8 SDK
  Docker (optional)

**Running Locally**
Clone the Repository:

**Bash**
  git clone https://github.com/your-username/UserTaskJWT.git
  cd UserTaskJWT

### Install Dependencies:

**Bash**
  dotnet restore

### Run the Project:

Using HTTP:

**Bash**
  dotnet run

The API will be accessible at http://localhost:5000 (or another port if specified in configuration).

Using Docker Compose:

**Bash**
  docker-compose up -d

The API will be accessible at https://localhost:9901 (or the port specified in the docker-compose.yml file).

## API Documentation
Swagger UI: Once the server is running, access the Swagger UI at https://localhost:9901/swagger to explore and interact with the API endpoints.
## Architecture and Design Choices

### ER DB Diagram
![UserTaskJwtER drawio](https://github.com/user-attachments/assets/430d3a6c-52de-48a5-8d26-ae39fec40584)


### Minimal API with Clean Architecture
**Minimal API**: We've chosen Minimal APIs in .NET 8 for their simplicity and focus on essential features. This reduces boilerplate code and improves development speed.

### Repository Pattern
**Repository Pattern**: The Repository Pattern is used to abstract data access logic, making it easier to switch between different data storage implementations (e.g., in-memory, SQL Server, NoSQL databases) without affecting the core application logic.
## Future Improvements
Due to time constraints, some classes could be further refined:

**Infrastructure Project**: Classes like PasswordHasher and Paged extension methods ideally belong in a separate Infrastructure project to maintain a clear separation of concerns.
