# DeDottifyHeader

A .NET 9 console application that cleans and standardizes CSV file headers by removing or replacing unwanted characters like dots, spaces, and hyphens.

## Overview

DeDottifyHeader is a utility tool designed to process CSV files and clean their headers according to configurable replacement rules. This is particularly useful when working with CSV files that have headers containing characters that might cause issues in data processing pipelines, databases, or analytics tools.

## Features

- **Configurable String Replacements**: Define custom find-and-replace rules for header cleaning
- **Multiple Column Separators**: Support for pipe (`|`), comma (`,`), tab (`\t`), semicolon (`;`), and other separators
- **Recursive Directory Processing**: Process all CSV files in a directory and its subdirectories
- **File Pattern Matching**: Specify custom file patterns (default: `*.csv`)
- **In-Memory Processing**: Process CSV data without writing to disk
- **Comprehensive Logging**: Detailed console output showing what changes were made
- **Robust Error Handling**: Graceful handling of missing files and empty CSV files

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 or any compatible IDE

### Building the Project

1. Clone or download this repository
2. Open a terminal in the project directory
3. Restore dependencies and build:

```bash
dotnet restore
dotnet build
```

### Running the Application

1. Configure the `appsettings.json` file (see Configuration section below)
2. Run the application:

```bash
dotnet run --project DeDottifyHeader
```

## Configuration

The application uses an `appsettings.json` file for configuration. Here's the structure:

```json
{
  "ProcessingSettings": {
    "TargetDirectory": "C:\\path\\to\\your\\csv\\files",
    "FilePattern": "*.csv",
    "IncludeSubdirectories": true,
    "ColumnSeparator": "|",
    "Replacements": [
      {
        "Find": ".",
        "Replace": ""
      },
      {
        "Find": " ",
        "Replace": "_"
      },
      {
        "Find": "-",
        "Replace": "_"
      }
    ]
  }
}
```

### Configuration Options

| Setting | Description | Default |
|---------|-------------|---------|
| `TargetDirectory` | Directory containing CSV files to process | Required |
| `FilePattern` | File pattern to match | `*.csv` |
| `IncludeSubdirectories` | Process subdirectories recursively | `true` |
| `ColumnSeparator` | Character used to separate columns | `|` |
| `Replacements` | Array of find-and-replace rules | `[]` |

### Replacement Rules

Each replacement rule consists of:
- `Find`: The string to search for in column headers
- `Replace`: The string to replace it with (can be empty for removal)

## Examples

### Example 1: Basic Dot and Space Removal

**Input CSV Header:**
```
First.Name|Last.Name|Email.Address|Phone Number
```

**Configuration:**
```json
{
  "Replacements": [
    { "Find": ".", "Replace": "" },
    { "Find": " ", "Replace": "_" }
  ]
}
```

**Output:**
```
FirstName|LastName|EmailAddress|Phone_Number
```

### Example 2: Working with Comma-Separated Files

**Input CSV Header:**
```
First.Name,Last-Name,Email Address,Phone.Number
```

**Configuration:**
```json
{
  "ColumnSeparator": ",",
  "Replacements": [
    { "Find": ".", "Replace": "" },
    { "Find": " ", "Replace": "_" },
    { "Find": "-", "Replace": "_" }
  ]
}
```

**Output:**
```
FirstName,Last_Name,Email_Address,PhoneNumber
```

## Project Structure

```
DeDottifyHeader/
??? DeDottifyHeader/              # Main console application
?   ??? Program.cs                # Entry point and main logic
?   ??? CsvHeaderProcessor.cs     # Core processing logic
?   ??? ProcessingSettings.cs     # Configuration model
?   ??? AppSettings.cs           # Application settings model
?   ??? StringReplacement.cs     # Replacement rule model
?   ??? appsettings.json         # Configuration file
?   ??? DeDottifyHeader.csproj   # Project file
??? Test/                        # Unit tests
?   ??? Test1.cs                 # Comprehensive test suite
?   ??? Test.csproj              # Test project file
??? README.md                    # This file
```

## API Usage

You can also use the `CsvHeaderProcessor` class directly in your own applications:

```csharp
using DeDottifyHeader;

var processor = new CsvHeaderProcessor();
var settings = new ProcessingSettings
{
    ColumnSeparator = "|",
    Replacements = new List<StringReplacement>
    {
        new StringReplacement { Find = ".", Replace = "" },
        new StringReplacement { Find = " ", Replace = "_" }
    }
};

// Process a single header string
string cleanHeader = processor.CleanHeader("First.Name|Last Name", settings);
// Result: "FirstName|Last_Name"

// Process CSV data in memory
string[] csvLines = { "First.Name|Last Name", "John|Doe" };
string[] processedLines = processor.ProcessCsvFileInMemory(csvLines, settings);

// Process a file directly
processor.ProcessCsvFile("path/to/file.csv", settings);
```

## Testing

The project includes comprehensive unit tests covering:

- Header cleaning with various replacement rules
- Different column separators
- Edge cases (empty files, single columns, etc.)
- File processing operations
- Integration scenarios

Run tests with:

```bash
dotnet test
```

## Dependencies

- **Microsoft.Extensions.Configuration** - Configuration management
- **Microsoft.Extensions.Configuration.Binder** - Configuration binding
- **Microsoft.Extensions.Configuration.Json** - JSON configuration support
- **Microsoft.NET.Test.Sdk** (Test project) - Testing framework
- **MSTest** (Test project) - Microsoft testing framework

## Use Cases

This tool is particularly useful for:

- **Data Pipeline Preparation**: Cleaning CSV headers before importing into databases
- **ETL Processes**: Standardizing column names for consistent data processing
- **Analytics Tools**: Preparing data for tools that don't handle special characters well
- **Database Import**: Ensuring column names are valid database identifiers
- **API Integration**: Converting file-based data to API-friendly formats

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## License

This project is open source. Feel free to use, modify, and distribute according to your needs.