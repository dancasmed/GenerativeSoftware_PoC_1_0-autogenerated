# UnitConverter

## Overview
Interactive distance conversion module supporting kilometers, miles, meters, and feet. Provides bidirectional conversions with history tracking and JSON persistence. Implements input validation and handles edge cases including zero/negative values through a console interface.

## Features
### Implemented Features
- ✅ Kilometer ↔ Mile conversions
- ✅ Meter ↔ Foot conversions
- 🛡️ Basic numeric input validation
- 📊 Conversion result display
- 📚 Conversion history persistence (JSON)
- ⏳ Historical record tracking with timestamps

### Planned Features
- 🔄 Full cross-unit conversions between all supported types
- 📖 Enhanced conversion history browser
- 🔄 Unit swap functionality
- 🎯 Decimal value support
- 🌐 Additional unit types (inches, yards, nautical miles)
- ⚡ Real-time conversion updates
- 📈 Visual conversion graphs
- 💾 Preset saving/loading

## Requirements
**Core Dependency:**
- `System.Text.Json` for JSON serialization

## Usage
1. **Main Menu**

1. Convert distance
2. Show conversion history
3. Exit

2. **Conversion Flow**
   - Enter numeric value
   - Select source unit (1-4)
   - Select target unit (1-4)
3. **History Management**
   - Automatic JSON storage in `dataFolder`
   - Timestamped entries with original/converted values

**Example Session:**

Enter value to convert: 100
Select source unit:
1. Kilometers
...
Converted: 100 kilometers = 62.15 miles


## Data Models
| Model                 | Properties                                                                 |
|-----------------------|----------------------------------------------------------------------------|
| **ConversionRequest** | `Value: float`, `SourceUnit: string`, `TargetUnit: string`                 |
| **ConversionResult**  | `OriginalValue: float`, `OriginalUnit: string`, `ConvertedValue: float`, `ConvertedUnit: string` |
| **ConversionHistory** | `Timestamp: DateTime`, `Request: ConversionRequest`, `Result: ConversionResult` |

## Roadmap
- Expand unit conversion matrix
- Implement unit swap shortcut
- Develop graphical visualization
- Enhance history management
- Add preset configuration system