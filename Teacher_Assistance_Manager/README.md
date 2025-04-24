# Teacher Assistance Manager

## Overview
Attendance management system designed to help teachers manage student groups, record daily attendance, and generate weekly summaries with automatic percentage calculations (Version 0.1.4). Provides a console-based interface for handling all attendance-related tasks while ensuring data integrity.

## Features

### Implemented Features
- Create and manage student groups
- Add/edit/delete students in groups
- Mark daily attendance per group
- View daily attendance records
- Generate weekly attendance percentage summaries

### Future/Planned Features
* Bulk import/export of student data
* Customizable attendance statuses (present/absent/excused)
* Record filtering by date/student
* Attendance trend visualizations
* Low attendance notifications
* Predictive analytics
* School calendar integration
* Multi-teacher collaboration

## Requirements
**Dependencies:**
- .NET Framework
- System.Text.Json API
- System.IO API

## Usage
1. **Create Group** (Menu Option 1)
   - Add new groups and manage student rosters

2. **Record Attendance** (Menu Option 2)
   
   Select group → Choose date → Mark present/absent for each student
   

3. **View/Edit Records** (Menu Option 3)
   - Review attendance history by date/student
   - Modify existing records

4. **Generate Summary** (Menu Option 4)
   
   Select group → Choose week → System displays:
   - Weekly attendance percentage
   - Daily status breakdown
   - Student-level statistics
   

## Data Models

**Group**

{
  "group_id": "string",
  "group_name": "string",
  "creation_date": "date",
  "teacher_id": "string"
}


**Student**

{
  "student_id": "string",
  "first_name": "string",
  "last_name": "string",
  "group_id": "string"
}


**Attendance**

{
  "attendance_id": "string",
  "student_id": "string",
  "date": "date",
  "status": "string",
  "group_id": "string"
}


**WeeklySummary**

{
  "summary_id": "string",
  "group_id": "string",
  "week_start_date": "date",
  "week_end_date": "date",
  "total_students": "integer",
  "total_present": "integer",
  "percentage": "float"
}


## Roadmap
- Mobile-friendly attendance marking
- Automated admin reporting
- Enhanced data visualization capabilities
- Advanced pattern recognition for attendance trends
- Multi-user collaboration features