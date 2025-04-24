# ExpenseTracker

## Overview
Expense tracking module with user authentication, CRUD operations for expenses, dynamic category management, and monthly financial reporting. Stores data in JSON format with password hashing security. Designed to help users monitor spending patterns through categorized expense tracking and report generation.

## Features

### Implemented Features
- Basic expense logging with amount, date, and category
- Predefined system categories (Food, Transport, Housing)
- User registration/login with secure password hashing
- Expense management (add/view/edit/delete)
- Monthly expense summaries by category
- Custom category creation
- JSON data persistence

### Planned Features
- Advanced filtering/sorting of expenses
- Report export (PDF/CSV)
- Visual charts for expense analysis
- Recurring expense tracking
- Budgeting tools with spending alerts
- Bank account integration

## Requirements
- .NET Core runtime
- System.Text.Json package (included in .NET Core)
- System.Security.Cryptography (included in .NET Core)

## Usage
1. **Authentication**
   - Register new account or login with existing credentials
2. **Main Menu Options**
   - *Add Expense*: Enter amount, date, category, and optional notes
   - *View/Edit Expenses*: Display chronological list with edit/delete options
   - *Generate Report*: Select month/year to view categorized spending summary
   - *Manage Categories*: Create/edit/delete custom categories
3. **Data Management**
   - All changes auto-save to JSON files
   - System categories cannot be modified/deleted

## Data Models

| Model     | Fields                                  | Description                                  |
|-----------|-----------------------------------------|----------------------------------------------|
| **User**  | Id, Username, PasswordHash             | Stores authentication credentials            |
| **Category** | Id, Name, UserId                     | System default or user-defined classifications |
| **Expense** | Id, Amount, Date, CategoryId, Notes | Financial transactions with metadata         |
| **Report** | Month, Year, GeneratedAt              | Temporal expense summaries (in-memory)       |

## Roadmap
- Multi-format report exports
- Graphical data visualization
- Automated bank transaction imports
- Multi-user collaboration features
- Budget configuration with alerts
- Recurring expense templates