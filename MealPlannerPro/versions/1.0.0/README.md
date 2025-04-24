# MealPlannerPro

## Overview
A meal planning module for creating weekly meal plans, tracking calorie intake, and generating grocery lists. Supports basic CRUD operations for meals/plans and calculates nutritional totals. Current version focuses on core meal planning functionality with user settings management.

## Features

### Implemented
- Create and manage weekly meal plans
- Calculate and display daily/weekly calorie counts
- Generate basic grocery lists
- User profile management with calorie targets

### Planned
- Dietary preferences/restrictions input
- Grocery list organization by categories
- Portion size adjustments
- Meal plan saving/reuse
- Nutritional tracking (protein, carbs)
- Grocery delivery integrations
- Fitness tracker synchronization

## Requirements
No external NuGet packages or APIs required. Built with .NET standard libraries.

## Usage
text
1. Launch module to access main menu:
   - Manage meals/ingredients
   - Create weekly meal plans
   - Generate grocery lists
   - Configure user settings

2. Example workflow:
   a. Set calorie target in user settings
   b. Create meals with ingredients
   c. Assign meals to days in meal plan
   d. Generate grocery list from plan

3. Console menus guide through:
   - Meal creation/selection
   - Plan date ranges
   - User preference management
   - Data persistence between sessions


## Data Models
csharp
// Core entities
public class Meal {
  string Id
  string Name
  string Description
  List<Ingredient> Ingredients
  int Calories
  int PreparationTime
  List<string> DietaryTags
}

public class MealPlan {
  string Id
  string UserId
  DateTime StartDate
  DateTime EndDate
  List<Day> Days
}

public class GroceryList {
  string Id
  string MealPlanId
  List<GroceryItem> Items
  int TotalItems
}

public class User {
  string Id
  string Name
  List<string> DietaryPreferences
  List<string> DietaryRestrictions
  int CalorieTarget
}


## Roadmap
- Enhanced grocery list organization
- Meal customization features
- Nutritional tracking expansion
- Third-party service integrations
- Multi-user support for families
- AI-powered meal recommendations