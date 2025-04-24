# VinylRecordsManager

## Overview
A vinyl collection management system supporting CRUDS operations, search functionality, and basic reporting. Maintains collection data in JSON format with genre categorization and condition tracking. Current version: 0.1.2

## Features
**Implemented Features**
- Basic CRUDS operations for vinyl records
- Simple search by title/artist/genre
- Genre and artist categorization
- Core metadata tracking (title, artist, year, genre, condition, value)
- Collection summary reports
- Genre distribution reports
- Record value calculations

**Planned Features**
- Advanced search filters
- Custom tagging system
- Wishlist management
- Condition history tracking
- Barcode scanning integration
- Automatic metadata fetching from music databases
- Multi-user collaboration

## Requirements
- .NET runtime (version 6.0 or newer)
- SelfEvolvingSoftware.Interfaces package
- Persistent storage for JSON data files

## Usage
1. **Main Menu Options**:
   - Manage Records (Add/Edit/Delete/List/Search)
   - Generate Reports (Summary/Genre Distribution/Value Analysis)
2. **Adding a Record**:
   
   Select '1. Add Record'
   Enter: Title, Artist, Year, Genre
   Specify Condition (Mint/Good/Fair)
   Input estimated value
   
3. **Searching Records**:
   
   Choose search criteria (Title/Artist/Genre)
   Enter search term
   View filtered results
   
4. **Generating Reports**:
   
   Select report type
   View auto-generated statistics:
   - Total collection value
   - Genre distribution
   - Artist categorization
   

## Data Models
**Record**
- `Id`: Unique GUID identifier
- `Title`: Album/record title
- `Artist`: Performing artist/group
- `Year`: Release year
- `Genre`: Musical genre classification
- `Condition`: Physical state (Mint/Good/Fair)
- `Value`: Estimated market value

**Wishlist** *(Planned)*
- `Priority`: Acquisition priority level
- `TargetPrice`: Desired purchase price
- `ReleaseDetails`: Edition/version information

**Report**
- `Type`: Summary/Genre/Value/Categorization
- `GenerationDate`: Timestamp
- `Statistics`: Formatted report content

## Roadmap
- Implement wishlist management system
- Add historical value tracking
- Develop advanced filtering capabilities
- Create export functionality (CSV/PDF)
- Integrate with Discogs API for metadata
- Add multi-user access controls
- Implement backup/restore features