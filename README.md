# Personal Data Management System
Full-stack web application built with Angular 14 and .NET 9

## 🚀 Tech Stack

### Frontend
- **Angular 14**
- **Angular Material**  
- **Bootstrap 5.3**
- **TypeScript**

### Backend  
- **.NET 9**
- **CQRS + MediatR** - Command Query Responsibility Segregation
- **Entity Framework Core** - ORM for database operations
- **RESTful API** - Web API architecture

## 📊 Database Schema
Persons Table
| Column Name | Data Type     | Constraints                       | Description               |
|-------------|---------------|-----------------------------------|---------------------------|
| Id          | int           | Primary Key, Identity             | Unique identifier         |
| FirstName   | nvarchar(50)  | NOT NULL                          | Person's first name       |
| LastName    | nvarchar(50)  | NOT NULL                          | Person's last name        |
| Address     | nvarchar(500) | NOT NULL                          | Full address              |
| BirthDate   | date          | NOT NULL                          | Date of birth             |
| Age         | int           | NOT NULL                          | Calculated age            |
| CreatedAt   | datetime2     | NOT NULL, Default: GETUTCDATE()   | Record creation timestamp |
| UpdatedAt   | datetime2     | NULL                              | Last update timestamp     |
