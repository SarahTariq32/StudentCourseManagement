# StudentCourseManagement

A Student Course Management Web API built with **ASP.NET Core 8**, **Entity Framework Core**, and **Microsoft SQL Server**, following the principles of **Clean Architecture**.

The project demonstrates a layered backend architecture with separation of concerns between API, Application logic, Domain entities, Infrastructure, and **JWT-based Role-Based Access Control (RBAC)**.

---

## Tech Stack

- **.NET 8 / ASP.NET Core Web API**
- **C#**
- **Entity Framework Core 8**
- **Microsoft SQL Server** (Database First approach)
- **JWT (JSON Web Token) Authentication**
- **Role-Based Access Control (RBAC)**
- **Swagger** (with Bearer Token Authentication)
- **Repository Pattern & DTOs**
- **Clean Architecture**

---

## Architecture

The solution is structured into four Clean Architecture layers:

```
StudentCourseManagement/
├── StudentCourseManagement.API          # Web API Controllers, Auth & Middleware
├── StudentCourseManagement.Application  # Services, DTOs, Interfaces, Business Logic
├── StudentCourseManagement.Domain       # Entities & Core Domain Models
└── StudentCourseManagement.Infrastructure # EF Core DbContext, Repositories & Mappings
```

---

## Authentication & Role-Based Access Control (RBAC)

Authentication is handled via **JWT tokens**. The application supports two primary roles stored in the `UsersData` table (`Role` claim):

### 1. Admin Role (`Admin` / `admin`)
- **Full Access**: Has permission to execute all CRUD operations on both **Students** and **Courses** endpoints.

### 2. Student Role (`Student` / `student`)
- **Self-Service Access**: Allowed to view (`GET /api/Students/{id}`) and update (`PUT /api/Students/{id}`) **only their own student profile**.
- **Resource Ownership Verification**: Verification matches the logged-in token's username against the student entity's `Name` in a space-insensitive and case-insensitive manner (e.g. JWT username `"saratariq"` matches student name `"Sara Tariq"`).
- **Restricted Endpoints**:
  - Cannot view all students (`GET /api/Students`) -> Returns `403 Forbidden`
  - Cannot create new students (`POST /api/Students`) -> Returns `403 Forbidden`
  - Cannot delete students (`DELETE /api/Students/{id}`) -> Returns `403 Forbidden`
  - Cannot view or edit another student's record -> Returns `403 Forbidden`
  - Cannot access Course management (`/api/Courses`) -> Returns `403 Forbidden`

---

## API Endpoints

| Area | HTTP Method | Endpoint | Authorization | Description |
| :--- | :---: | :--- | :--- | :--- |
| **Auth** | `POST` | `/api/Auth/register` | Public | Register a new user |
| **Auth** | `POST` | `/api/Auth/login` | Public | Authenticate and obtain JWT token |
| **Courses** | `GET` | `/api/Courses` | Admin | Retrieve all courses |
| **Courses** | `GET` | `/api/Courses/{id}` | Admin | Retrieve course by ID |
| **Courses** | `POST` | `/api/Courses` | Admin | Create a new course |
| **Courses** | `PUT` | `/api/Courses/{id}` | Admin | Update an existing course |
| **Courses** | `DELETE` | `/api/Courses/{id}` | Admin | Delete a course |
| **Students** | `GET` | `/api/Students` | Admin | Retrieve all students |
| **Students** | `GET` | `/api/Students/{id}` | Admin / Student (Own profile) | Retrieve student by ID |
| **Students** | `POST` | `/api/Students` | Admin | Create a new student |
| **Students** | `PUT` | `/api/Students/{id}` | Admin / Student (Own profile) | Update student by ID |
| **Students** | `DELETE` | `/api/Students/{id}` | Admin | Delete a student |

---

## How to Run & Test

1. **Database Setup**: Ensure SQL Server is running and connection string in `appsettings.json` is configured.
2. **Build & Run**:
   ```bash
   dotnet build StudentCourseManagement.sln
   dotnet run --project StudentCourseManagement.API
   ```
3. **Swagger UI**:
   - Navigate to `https://localhost:<port>/swagger`.
   - Log in via `/api/Auth/login` to receive a JWT token.
   - Click the **Authorize** button in Swagger UI and paste your JWT token.
   - Test endpoints according to your assigned user role (`admin` or `student`).
