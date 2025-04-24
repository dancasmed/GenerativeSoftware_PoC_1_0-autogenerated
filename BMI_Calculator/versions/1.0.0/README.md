# BMI Calculator

## Overview
A software module that calculates Body Mass Index (BMI) using metric or imperial units. Validates user inputs, computes BMI, categorizes results into health categories, and provides recommendations. Each session is saved to a JSON file containing all inputs and results.

## Features
### Implemented Features
- Calculate BMI using standard formula (weight / height²)
- Support for metric units (kg, cm)
- Basic input validation (positive numbers, reasonable ranges)
- Display BMI result with health category (e.g., underweight, normal weight)
- Session persistence (saves inputs/results to JSON file)

### Planned Features
- Support imperial units (lbs, inches)
- Enhanced input validation (e.g., realistic height/weight ranges)
- Save user history for future reference
- Personalized health recommendations
- Multi-language support
- Integration with health tracking APIs
- Graphical BMI trend visualization

## Requirements
- .NET runtime (compatible with .NET Core 3.1 or later)
- No external NuGet packages required

## Usage
1. Run the module
2. Choose measurement system (metric/imperial)
3. Enter height and weight when prompted
4. View BMI result and health category
5. Optionally repeat or exit

**Example Interaction**:

Choose units (metric/imperial): metric
Enter height in centimeters: 175
Enter weight in kilograms: 68
BMI: 22.20
Category: Normal weight
Recommendation: Maintain a healthy diet and exercise routine.


## Data Models
- **UserInput**:
  - Height (float)
  - Weight (float)
  - Units (string: "metric" or "imperial")
- **BMIResult**:
  - BMI (float)
  - Category (string)
  - Recommendation (string)
- **UserSession**:
  - User ID (string)
  - Inputs (collection of UserInput)
  - Results (collection of BMIResult)
  - Timestamp (datetime)

## Roadmap
- Add support for imperial unit conversions
- Implement historical data analysis
- Create graphical output options
- Develop API integration capabilities
- Expand validation rules for medical plausibility
- Introduce multi-language interface support