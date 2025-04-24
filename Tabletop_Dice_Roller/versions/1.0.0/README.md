# Tabletop Dice Roller

## Overview
Advanced dice roller module for tabletop games with JSON data persistence. Supports all standard RPG dice types (d4-d100) with true randomness, enabling users to roll combinations, save favorite configurations, and track roll history.

## Features
**Implemented Features**
- Roll single or multiple dice of the same type
- Display individual dice values and total sum
- Support for standard dice types (d4, d6, d8, d10, d12, d20, d100)

**Planned Features**
- Roll combinations of different dice types in single action
- Save/retrieve frequently used dice combinations
- Roll history tracking
- Custom dice types (d3, d7 etc.)
- Advanced roll options (drop lowest/highest, reroll logic)
- Export capabilities for results/history

## Requirements
**Dependencies**
- Newtonsoft.Json (v13.0.3)

## Usage
1. **Roll Dice**
   - Select dice types from available options
   - Specify quantities for each type
   - Type 'done' to execute roll
   - Example:
     
     Enter dice type (or 'done'): d20
     Quantity: 2
     Enter dice type (or 'done'): d6
     Quantity: 4
     Enter dice type (or 'done'): done
     

2. **Save Combinations**
   - After creating a dice set, assign a name
   - Retrieve later using combination ID

3. **History**
   - View timestamps and roll counts
   - Full results persist between sessions

## Data Models
**DiceRoll**
- `Id`: Unique identifier (Guid)
- `DiceType`: Dice specification (e.g., "d20")
- `Quantity`: Number of dice rolled
- `Results`: Array of individual roll results
- `Total`: Sum of results
- `Timestamp`: Creation datetime

**SavedCombination**
- `Id`: Unique identifier (Guid)
- `Name`: User-defined label
- `DiceTypes`: Array of dice specifications
- `Quantities`: Corresponding quantities array

**RollHistory**
- `Id`: Unique identifier (Guid)
- `Rolls`: Collection of DiceRoll objects
- `Timestamp`: Session datetime

## Roadmap
- Implement combination rolls with mixed dice types
- Add history review with detailed breakdowns
- Introduce modifiers for complex RPG scenarios
- Develop export/import functionality
- Support custom dice configurations