# Enhanced TODO API with Querying

A production-quality RESTful API built with ASP.NET Core Web API and Entity Framework Core.
Implements advanced querying features including filtering, searching, sorting, and pagination,
backed by a persistent SQL Server LocalDB database. Built with clean architecture, DTOs,
comprehensive validation, standardized error handling, and async/await throughout.

## Tech Stack

| Technology | Purpose |
|---|---|
| C# / ASP.NET Core Web API | Core framework |
| .NET 8 | Runtime |
| Entity Framework Core | Database ORM |
| SQL Server LocalDB | Database |
| LINQ | Querying and filtering |
| Swagger / OpenAPI | API documentation & testing |
| Postman | API testing & collection |


## Project Structure

TODOAPI-QUERY/
├── Controllers/
│   └── TodoController.cs          # Handles HTTP requests & query parameters
├── Data/
│   └── AppDbContext.cs            # Entity Framework DB context & seeding
├── DTOs/
│   ├── CreateTodoDTO.cs           # Input DTO for creating todos
│   ├── UpdateTodoDTO.cs           # Input DTO for updating todos
│   └── TodoResponseDTO.cs         # Output DTO for API responses
├── Migrations/                    # Auto-generated EF Core migrations
├── Models/
│   └── TodoItem.cs                # Core data model
├── Responses/
│   └── ErrorResponse.cs           # Standardized error response format
├── Services/
│   ├── ITodoService.cs            # Service interface (contract)
│   └── TodoService.cs             # Business logic & async DB operations
├── appsettings.json               # App configuration & connection string
└── Program.cs                     # App entry point & service registration


## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- SQL Server LocalDB (included with Visual Studio)
- [SQL Server Management Studio](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) — optional

### Installation

**1. Clone the repository**
```bash
git clone https://github.com/peculiarprecious/TODOAPI-QUERY.git
```

**2. Navigate to project folder**
```bash
cd TODOAPI-QUERY/TODOAPI-QUERY
```

**3. Restore dependencies**
```bash
dotnet restore
```

**4. Install EF Core tools (if not already installed)**
```bash
dotnet tool install --global dotnet-ef
```

**5. Apply database migrations**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**6. Run the project**
```bash
dotnet run
```

**7. Open Swagger UI**
```
https://localhost:7180/swagger
```


## Database Setup

### Connection String
Configured in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TodoApiDb;Trusted_Connection=True;"
  }
}
```

### Migration Commands

| Command | Purpose |
|---|---|
| `dotnet ef migrations add InitialCreate` | Create migration from model |
| `dotnet ef database update` | Apply migration & create tables |

### Verify Data in SSMS
```
1. Open SQL Server Management Studio
2. Server name: (localdb)\mssqllocaldb
3. Authentication: Windows Authentication
4. Navigate: Databases → TodoApiDb → Tables → dbo.TodoItems
5. Right-click → Select Top 1000 Rows
```

Or run SQL directly:
```sql
USE TodoApiDb;
SELECT * FROM TodoItems;
```

## Database Seeding

The application automatically seeds **10 sample todo records** on first run
covering all priority levels (Low, Medium, High) and completion statuses,
giving you data to test all query features immediately.

| Seed Data | Count |
|---|---|
| Total todos | 10 |
| High priority | 3 |
| Medium priority | 4 |
| Low priority | 3 |
| Completed | 4 |
| Pending | 6 |


## API Endpoints

### Base URL
```
https://localhost:7136/api/Todo
```

### Endpoint Summary

| Method | Endpoint | Description | Status Codes |
|--------|----------|-------------|--------------|
| `GET` | `/api/Todo` | Get all todos with filtering, sorting & pagination | 200 |
| `GET` | `/api/Todo/{id}` | Get single todo by ID | 200, 404 |
| `GET` | `/api/Todo/search` | Search todos by keyword | 200 |
| `GET` | `/api/Todo/stats` | Get todo statistics | 200 |
| `POST` | `/api/Todo` | Create new todo | 201, 400 |
| `PUT` | `/api/Todo/{id}` | Update existing todo | 200, 400, 404 |
| `DELETE` | `/api/Todo/{id}` | Delete todo | 204, 404 |


## Query Parameters

### GET /api/Todo — Query Parameters

| Parameter | Type | Description | Example |
|---|---|---|---|
| `status` | string | Filter by: `completed` or `pending` | `?status=completed` |
| `priority` | string | Filter by: `Low`, `Medium`, `High` | `?priority=High` |
| `sortBy` | string | Sort by: `title`, `duedate`, `createdat`, `priority` | `?sortBy=title` |
| `sortOrder` | string | Sort direction: `asc` or `desc` | `?sortOrder=desc` |
| `page` | int | Page number (default: 1) | `?page=1` |
| `pageSize` | int | Items per page (default: 10) | `?pageSize=10` |

### GET /api/Todo/search — Query Parameters

| Parameter | Type | Description | Example |
|---|---|---|---|
| `q` | string | Search keyword in title and description | `?q=meeting` |


## Request & Response Examples

### POST /api/Todo — Create Todo

**Request**
```http
POST /api/Todo
Content-Type: application/json

{
  "title": "Buy groceries",
  "description": "Milk, eggs and bread",
  "dueDate": "2026-12-01T00:00:00",
  "priority": "High"
}
```

**Response** `201 Created`
```json
{
  "id": 1,
  "title": "Buy groceries",
  "description": "Milk, eggs and bread",
  "isCompleted": false,
  "createdAt": "2026-05-16T10:30:00",
  "dueDate": "2026-12-01T00:00:00",
  "priority": "High"
}
```

### GET /api/Todo — Get All (with pagination)

**Response** `200 OK`
```json
{
  "data": [
    {
      "id": 1,
      "title": "Buy groceries",
      "description": "Milk, eggs and bread",
      "isCompleted": false,
      "createdAt": "2026-05-16T10:30:00",
      "dueDate": "2026-12-01T00:00:00",
      "priority": "High"
    }
  ],
  "totalCount": 10,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

---

### Error Response `400 Bad Request`
```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": {
    "Title": ["Title is required"],
    "DueDate": ["DueDate cannot be in the past"]
  },
  "timestamp": "2026-05-16T10:30:00"
}
```

### Error Response `404 Not Found`
```json
{
  "statusCode": 404,
  "message": "Todo with id 999 not found",
  "errors": null,
  "timestamp": "2026-05-16T10:30:00"
}
```

---

## 🔎 Query Examples

### Filtering Examples
```
GET /api/Todo?status=completed
GET /api/Todo?status=pending
GET /api/Todo?priority=High
GET /api/Todo?status=pending&priority=High
```

### Search Examples
```
GET /api/Todo/search?q=meeting
GET /api/Todo/search?q=grocery
```

### Sorting Examples
```
GET /api/Todo?sortBy=title&sortOrder=asc
GET /api/Todo?sortBy=title&sortOrder=desc
GET /api/Todo?sortBy=duedate&sortOrder=asc
GET /api/Todo?sortBy=duedate&sortOrder=desc
GET /api/Todo?sortBy=createdat&sortOrder=desc
```

### Pagination Examples
```
GET /api/Todo?page=1&pageSize=10
GET /api/Todo?page=2&pageSize=5
GET /api/Todo?page=2&pageSize=20
```

### Combined Query Examples
```
# High priority sorted by due date, page 1
GET /api/Todo?priority=High&sortBy=duedate&sortOrder=asc&page=1&pageSize=10

# Pending todos sorted by title, 5 per page
GET /api/Todo?status=pending&sortBy=title&sortOrder=asc&page=1&pageSize=5

# Completed high priority sorted by created date
GET /api/Todo?status=completed&priority=High&sortBy=createdat&sortOrder=desc

# All todos page 2 with 20 items
GET /api/Todo?page=2&pageSize=20
```


## Statistics Endpoint

### GET /api/Todo/stats

**Response** `200 OK`
```json
{
  "totalTodos": 10,
  "completedTodos": 4,
  "pendingTodos": 6,
  "highPriority": 3,
  "mediumPriority": 4,
  "lowPriority": 3,
  "completionRate": "40%"
}
```

## Validation Rules

| Field | Rules |
|---|---|
| `Title` | Required, 3–100 characters, no whitespace only |
| `Description` | Required(Cannot be empty), max 500 characters |
| `Priority` | Must be `Low`, `Medium`, or `High` |
| `DueDate` | Cannot be in the past |


## HTTP Status Codes

| Code | Meaning |
|---|---|
| `200 OK` | Request successful |
| `201 Created` | Todo created successfully |
| `204 No Content` | Todo deleted successfully |
| `400 Bad Request` | Validation failed |
| `404 Not Found` | Todo not found |
| `500 Internal Server Error` | Server error |



## Async Implementation

All database operations use async/await for non-blocking performance:

```csharp
await _context.Todos.ToListAsync()     // fetch all records
await _context.Todos.FindAsync(id)     // find by primary key
await _context.SaveChangesAsync()      // save changes
```


## Architecture

```
HTTP Request
     ↓
Controller    → validates input, handles HTTP & query params
     ↓
ITodoService  → interface contract
     ↓
TodoService   → business logic & LINQ queries
     ↓
AppDbContext  → Entity Framework Core
     ↓
SQL Server LocalDB
```



## 👩‍💻 Author

**Precious Nwajei**
- GitHub: [@peculiarprecious](https://github.com/peculiarprecious)
