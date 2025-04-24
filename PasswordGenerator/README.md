# PasswordGenerator

## Overview
Secure password generator module with customizable criteria and history tracking. Generates cryptographically secure passwords with configurable length and character sets, performs strength evaluation, and maintains JSON-based password history.

## Features

### Implemented Features
- Generate random passwords of specified length (8-64 characters)
- Option to include/exclude special characters
- Basic security checks for password strength
- Password history tracking in JSON format
- Strength evaluation (Weak, Medium, Strong, Very Strong)

### Planned Features
- Additional character set options (numbers/uppercase/lowercase toggles)
- Password strength indicator visualization
- History export capabilities
- Customizable character sets
- Passphrase generation support
- Password manager integration

## Requirements
- .NET 5.0+ runtime
- System.Text.Json for serialization
- Cryptographically secure random number generator (System.Security.Cryptography)

## Usage

1. Launch module and select 'Generate New Password'
2. Enter desired password length (8-64 characters)
3. Answer Y/N for special characters inclusion
4. Receive generated password with strength rating
5. Weak passwords trigger regeneration prompt
6. All generated passwords stored in password_history.json


## Data Models

### PasswordCriteria
typescript
{
  length: number (8-64),
  include_special_chars: boolean,
  include_numbers: boolean,
  include_uppercase: boolean,
  include_lowercase: boolean
}


### GeneratedPassword
typescript
{
  password: string,
  strength: string (Weak/Medium/Strong/Very Strong),
  timestamp: ISO8601 datetime
}


### PasswordHistory
typescript
{
  passwords: Array<GeneratedPassword>,
  user_id: string
}


## Roadmap
- Implement granular character set controls
- Add password strength visualization
- Develop passphrase generation mode
- Create history export functionality
- Add multi-user support
- Implement common password dictionary checks