# cleanarch-zest

# Product Management System - RESTful API

A high-performance CRUD API built with **.NET 8**, following **Clean Architecture** principles and the **Repository Pattern**. This project is designed to be scalable, testable, and fully containerized using Docker.

---

## 🏗 High-Level Architecture
The solution follows a **Clean Architecture** (Onion Architecture) approach to ensure a strict separation of concerns:

* **Domain**: Contains core entities (`Product`, `Item`), Enums, and custom Domain Exceptions. It has no dependencies on other layers.
* **Application**: Contains business logic, Service interfaces, DTOs, AutoMapper profiles, and FluentValidation rules.
* **Infrastructure**: Handles data access using **Entity Framework Core**, Repository implementations, Unit of Work, and JWT Identity services.
* **API**: The entry point of the application, containing Controllers, Middleware (Global Error Handling), and API Versioning.



---

## 🛠 Tech Stack
| Component | Technology |
| :--- | :--- |
| **Framework** | .NET 8 (C#) |
| **API Framework** | ASP.NET Core Web API |
| **Database** | SQL Server |
| **ORM** | Entity Framework Core |
| **Security** | JWT with Refresh Token Strategy |
| **Validation** | FluentValidation |
| **Testing** | xUnit, Moq, WebApplicationFactory |
| **Documentation** | Swagger / OpenAPI |
| **Containerization** | Docker & Docker Compose |

---

## 🔐 Authentication & Security
The API implements a robust security layer:
* **JWT Authentication**: Secure access using short-lived Bearer tokens.
* **Refresh Token Rotation**: Ensures users stay authenticated securely without re-logging, with one-time-use refresh tokens stored in the database.
* **Role-Based Access**: Specialized endpoints are protected based on user roles.
* **Input Validation**: All requests are sanitized and validated using FluentValidation before reaching the service layer.



---

## 🚀 Getting Started (Docker Deployment)

### Prerequisites
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed.
* Git installed.

### Installation & Setup
1.  **Clone the Repository**:
    ```bash
    git clone [https://github.com/MrJaber-07/ZestDockerDeploymentAssigment.git](https://github.com/MrJaber-07/ZestDockerDeploymentAssigment.git)
    cd ZestDockerDeploymentAssigment
    ```

2.  **Run with Docker Compose**:
    This command builds the API image and pulls the SQL Server image, setting up the network and volumes automatically.
    ```bash
    docker-compose up --build
    ```

3.  **Access the API Documentation**:
    Once the containers are running, navigate to:
    * **Swagger UI**: `http://localhost:5000/swagger`

---

## 🧪 Testing Strategy
The project includes automated tests to ensure reliability:
* **Unit Tests**: Located in `tests/Application.Tests`, focusing on business logic using **Moq**.
* **Integration Tests**: Located in `tests/API.Tests`, verifying API endpoints using **WebApplicationFactory**.

To run the tests locally:
```bash
dotnet test
