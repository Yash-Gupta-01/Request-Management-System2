# Request Management System (RMS)

A modern web application for managing and tracking requests within an organization. Built with **ASP.NET Core 8.0** and **Tailwind CSS**, this system provides an intuitive interface for users to submit requests and administrators to manage them.

## Features

- **User Authentication**: Secure login and registration system
- **Role-Based Access**: Different interfaces for guests and administrators
- **Request Management**: 
  - Create new requests with type, subject, and details  
  - Track request status (Yet to Start, In Progress, About to Finish, Finished)  
  - View active and completed requests  
  - Reactivate completed requests
- **Status History**: Complete audit trail of request status changes
- **Modern UI**: Responsive design using Tailwind CSS

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download) or later  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or higher)  
- [Node.js](https://nodejs.org/) (for Tailwind CSS)  
- [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio 2022](https://visualstudio.microsoft.com/)

## Installation Steps

### 1. Clone the Repository

    ```bash
    git clone <repository-url>
    cd RMS
    ```

### 2. Install .NET Dependencies

    ```bash
    dotnet restore
    ```

### 3. Install Node.js Dependencies

    ```bash
    npm install
    ```

### 4. Setup Database

    ```bash
    dotnet ef database update
    ```

### 5. Install and Configure Tailwind CSS

    ```bash
    npm install -D tailwindcss@latest
    npx tailwindcss init
    ```

Ensure `tailwind.config.js` contains:

    ```js
    module.exports = {
      content: [
        "./Views/**/*.cshtml",
        "./Views/**/*.html"
      ],
      theme: {
        extend: {},
      },
      plugins: [],
    }
    ```

### 6. Configure Database Connection

- Open `appsettings.json`  
- Update the connection string if needed:

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RMS;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

## Running the Application

1. **Start Tailwind CSS Watch Process**

    ```bash
    npx tailwindcss -i ./wwwroot/css/site.css -o ./wwwroot/css/styles.css --watch
    ```

2. **Run the Application**

    ```bash
    dotnet run
    ```

3. **Access in Browser**

- https://localhost:7205  
- http://localhost:5193

### Default Admin Credentials

- **Username**: `admin`  
- **Password**: `Admin@123`

## Common Issues and Solutions

### Database Connection Errors

- Verify SQL Server is running.  
- Check your connection string in `appsettings.json`.  
- Run:

    ```bash
    dotnet ef database update
    ```

### CSS Not Updating

- Ensure the Tailwind watch process is running.  
- Clear your browser cache.  
- Check file paths in `tailwind.config.js`.

### Login Issues

- Verify the database has been seeded with the initial admin user.  
- Run:

    ```sql
    SELECT * FROM Users;
    ```

### Request Creation Issues

- Verify the user is logged in as a guest.  
- Ensure form field names match controller parameters.  
- Make sure all required fields are filled.

## Development Guidelines

### Adding New Features

- Follow the existing project structure.  
- Use Tailwind CSS for styling.  
- Implement proper logging where necessary.  
- Add appropriate input validations.

### Database Changes

- Create a new migration:

    ```bash
    dotnet ef migrations add MigrationName
    ```

- Update the database:

    ```bash
    dotnet ef database update
    ```

## Project Structure

    RMS/
    ├── Controllers/          # MVC Controllers
    ├── Models/               # Data Models
    ├── Views/                # Razor Views
    ├── Data/                 # Database Context
    ├── wwwroot/              # Static Files
    │   ├── css/              # CSS Files
    │   └── js/               # JavaScript Files
    └── Migrations/           # Database Migrations

## Security Considerations

- All passwords are securely hashed using strong algorithms.  
- Role-based access control (RBAC) is implemented.  
- Session management is enabled for authentication.  
- Input validation on all forms.  
- SQL Injection protection via Entity Framework.
