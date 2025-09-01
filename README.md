# Library Management System API

A RESTful API designed to manage library operations, including book cataloging, borrowing, returning, and user management. Built using a clean 3-tier architecture for modularity and scalability.
## Architecture Overview

The application follows a Three-Tier Architecture, comprising:

1. Presentation Layer – Exposes RESTful endpoints for interaction   
2. Business Logic Layer – Handles the core functionality and rules  
3. Data Access Layer – Manages database interactions                                

## Technologies Used

- Language: C#  
- Framework: .NET Core , ADO.net, ASP.net
- Architecture: Three-Tier  
- Database: (SQL Server)

## Getting Started

### Prerequisites

- Visual Studio 2022 or later  
- .NET Framework (4.8)  
- SQL Server installed and running
- Access to SQL Server Management Studio (SSMS) or any SQL query tool

### Installation

1. Clone the repository  
  https://github.com/muzamilalfatih/library-management-system-API.git
2. Steps to Set Up the Database

    1. Open the schema.sql and run it.
    2. Open the seed.sql and run it.

3. Open the solution  
   Open the .sln file in Visual Studio.

4. Restore NuGet packages  
   Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution and restore missing packages.
5. Update connection string
6. Build and run  
   Use Build > Build Solution and Debug > Start Debugging to launch the application.
7. Use the following test credentials :

    1. Username : admin.
    2. Password : admin.

## Features

- Book Management:
    - Create, update, delete, and retrieve books.
    - Search and filter books by title, author, and category.
    - Pagination and sorting support.
- User Management:
    - Register, update, and delete users.
    - Assign roles (e.g., Admin, Librarian, Member).  
- Transaction Management:
    - Issue and return books.
    - Track due dates and overdue items.
    - Calculate fines for overdue books.
- Authentication & Authorization:
    - JWT-based authentication.
    - Role-based access control

## Contributing

Contributions are welcome!

1. Fork the repo  
2. Create a branch: git checkout -b feature/YourFeature  
3. Commit your changes: git commit -m "Add feature"  
4. Push the branch: git push origin feature/YourFeature  
5. Open a Pull Request

## Contact

If you have any questions or feedback, feel free to reach out:

- Email: muzamilalfatih123@gmail.com 
- GitHub: https://github.com/muzamilalfatih
