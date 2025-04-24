# Bookstore Inventory Manager

## Overview
Comprehensive inventory management system for bookstores supporting CRUD operations, sales tracking, restock alerts, and reporting. Maintains inventory data in JSON format with configurable stock thresholds and supplier/category management.

## Features

### Implemented Features
- Basic CRUD operations for books
- Sales transaction tracking with date/quantity/price
- Restock alerts when inventory falls below threshold
- Inventory status reporting
- Threshold adjustment functionality

### Planned Features
- Bulk import/export capabilities
- Advanced sales trend analysis
- Multi-level threshold alerts (warning/critical)
- Supplier system integration
- Predictive restocking algorithms
- Real-time inventory dashboard

## Requirements

**Dependencies**:
- .NET System APIs:
  - System.Collections.Generic
  - System.IO
  - System.Text.Json

## Usage

**Main Menu Options**:
1. Add/Update/Delete books
2. View/Search inventory
3. Record sales transactions
4. Monitor restock alerts
5. Generate inventory reports
6. Adjust stock thresholds

**Example Workflow**:

1. Add Book
   → Enter ISBN: 978-0451524935
   → Title: 1984
   → Author: George Orwell
   → Price: 9.99
   → Initial Stock: 50
   → Threshold: 10

2. Record Sale
   → ISBN: 978-0451524935
   → Quantity: 3
   → Auto-deducts stock
   → Generates sales record


## Data Models

**Core Entities**:
- **Book**: 
  - ISBN, Title, Author
  - Price, CurrentStock
  - MinimumStockThreshold
  - Dates: Added/Updated

- **Sale**:
  - Book reference
  - Quantity/Price
  - Sale date
  - Total calculation

- **RestockAlert**:
  - Triggered book
  - Stock levels
  - Alert status
  - Date generated

- **Category/Supplier**:
  - Organizational metadata
  - Contact/lead time info

## Roadmap

**Future Improvements**:
- Automated purchase order generation
- Customer purchase history tracking
- Multi-store inventory management
- Advanced report exporting (PDF/Excel)
- Barcode scanning integration
- Mobile-friendly interface