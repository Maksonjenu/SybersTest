# Project Management System (Test Assignment)

A web application for managing projects and employees, built with **ASP.NET Core** and **Entity Framework Core**. The application features a 3-tier architecture, a multi-step wizard for project creation, and AJAX-based interactions.

## Tech Stack

*   **Framework:** .NET 10 (ASP.NET Core MVC & Web API)
*   **Database:** SQLite (Entity Framework Core Code First)
*   **Frontend:** Razor Views, Bootstrap 5, **Vanilla JavaScript** (ES6+)

Include 3-Tier Architecture, DTO Pattern, AJAX, Asynchronous Programming.

## Key Features

### 1. Project Management (The Wizard)
A custom **5-step Wizard** for creating and editing projects:
1.  **General Info:** Validation of start/end dates.
2.  **Companies:** Customer and Executor details.
3.  **Project Manager:** AJAX search and selection from existing employees.
4.  **Team:** **Many-to-Many** relationship management. Search and add multiple employees to the project via AJAX.
5.  **Documents:** Drag & Drop file upload support.

### 2. Employee Management
*   Full CRUD (Create, Read, Update, Delete) for employees.
*   **AJAX-based operations:** Adding, editing, and deleting employees is done via API calls without full page reloads.
*   Search functionality.

### 3. Architecture & Code Quality
*   **3-Tier Architecture:**
    *   **Web:** Controllers, ViewModels, Views, API Endpoints.
    *   **Services:** Business logic, DTO mappings, File handling.
    *   **Data (Core/Infrastructure):** Database Context, Entities, Migrations.
*   **DTOs:** Used for data transfer to prevent over-posting and circular reference issues in JSON.
*   **Validation:** Server-side (Data Annotations) and Client-side validation.

## Configuration & Setup

The project uses **SQLite**, you don't need to install SQL Server. The database file will be created automatically.

### Prerequisites
*   .NET SDK (10.0) installed.

### How to Run


1.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```

2.  **Apply Migrations (Create Database):**
    *   This step creates the `siberstest.db` file based on the Entity Framework migrations.
    ```bash
    dotnet ef migrations add AddAttachedFiles --project SibersTest.Infrastructure --startup-project SibersTest.Web
    dotnet ef database update --startup-project SibersTest.Web
    ```
    

3.  **Run the Application:**
    ```bash
    dotnet run --project SibersTest.Web
    ```

4.  **Open in Browser:**
    Navigate to `https://localhost:5146` (or the port shown in the terminal).

## Project Structure

*   `SibersTest.Core` / `Data`: Contains Database Context (`ApplicationDbContext`) and Domain Entities (`Project`, `Employee`, `ProjectEmployee`).
*   `SibersTest.Services`: Contains Business Logic Interfaces (`IProjectService`) and Implementations. Handles mapping between Entities and DTOs.
*   `SibersTest.Web`:
    *   `Controllers/ProjectsController.cs`: MVC controller for navigation.
    *   `Controllers/Api/ProjectsApiController.cs`: REST API for handling Wizard logic (AJAX).
    *   `wwwroot/js`: Client-side logic for the Wizard and Employee management.

## Notes for the Reviewer

*   **File Uploads:** Uploaded documents are stored in `wwwroot/uploads`.
*   **Search:** Project search filters by Project Name, Customer Company, or Executor Company.
*   **Validation:** Start Date cannot be later than End Date (validated on both client and server).
*   **Concurrency:** Async/Await pattern is used throughout the application for I/O operations.
