# Teacher Assistance Manager

**Version:** 0.1.0

## Overview
Attendance management system for teachers to handle student groups, record daily attendance, and generate weekly reports. Implements CRUD operations for groups, students, and attendance records with JSON persistence.

## Features
✅ **Implemented Features**
- Create and manage student groups
- Add and remove students from groups
- Record daily attendance
- Generate weekly attendance summary reports

🛠 **Future Features**
- Bulk import/export of student data
- Customizable attendance statuses (e.g., present, absent, late)
- Filter and search functionality
- Basic analytics
- Calendar system integration
- Predictive analytics for at-risk students

## Requirements
No external dependencies required (uses .NET System.Text.Json for data persistence)

## Usage

1. Main Menu Options:
   - [1] Manage Groups (create/list/edit/delete)
   - [2] Manage Students (add/list/edit/remove)
   - [3] Record Daily Attendance
   - [4] Generate Weekly Report
   - [5] Exit

2. Example Workflow:
   - Create group: Provide name/description
   - Add students: Enter name/email/group ID
   - Record attendance: Select group, date, and mark status per student
   - Generate reports: Automatic weekly percentage calculations


## Data Models
**Group**  
`GroupId`, GroupName, Description, CreatedAt, UpdatedAt  

**Student**  
`StudentId`, `GroupId`, Name, Email, CreatedAt, UpdatedAt  

**Attendance**  
`AttendanceId`, `GroupId`, `StudentId`, Date, Status, Notes, CreatedAt, UpdatedAt  

**WeeklySummary**  
`SummaryId`, `GroupId`, WeekStartDate, WeekEndDate, TotalStudents, TotalPresent, TotalAbsent, AttendancePercentage

## Roadmap
- Enhanced data import/export capabilities
- Mobile-friendly interface
- Multi-teacher collaboration features
- Advanced attendance pattern analysis