using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using DeDottifyHeader;

namespace DeDottifyHeader.Tests
{
    public class CsvHeaderProcessorTests
    {
        private CsvHeaderProcessor _processor;

        public CsvHeaderProcessorTests()
        {
            _processor = new CsvHeaderProcessor();
        }

        [Fact]
        public void CleanHeader_WithNoReplacements_ReturnsOriginalHeader()
        {
            // Arrange
            string originalHeader = "Name.First|Age.Years|City.Location";
            var settings = new ProcessingSettings
            {
                ColumnSeparator = "|",
                Replacements = new List<StringReplacement>()
            };

            // Act
            string result = _processor.CleanHeader(originalHeader, settings);

            // Assert
            Assert.Equal(originalHeader, result);
        }

        [Fact]
        public void CleanHeader_WithDotReplacement_RemovesDots()
        {
            // Arrange
            string originalHeader = "Name.First|Age.Years|City.Location";
            var settings = new ProcessingSettings
            {
                ColumnSeparator = "|",
                Replacements = new List<StringReplacement>
                {
                    new StringReplacement { Find = ".", Replace = "" }
                }
            };

            // Act
            string result = _processor.CleanHeader(originalHeader, settings);

            // Assert
            Assert.Equal("NameFirst|AgeYears|CityLocation", result);
        }

        [Fact]
        public void CleanHeader_WithMultipleReplacements_AppliesAllReplacements()
        {
            // Arrange
            string originalHeader = "Name.First|Age Years|City-Location";
            var settings = new ProcessingSettings
            {
                ColumnSeparator = "|",
                Replacements = new List<StringReplacement>
                {
                    new StringReplacement { Find = ".", Replace = "" },
                    new StringReplacement { Find = " ", Replace = "_" },
                    new StringReplacement { Find = "-", Replace = "_" }
                }
            };

            // Act
            string result = _processor.CleanHeader(originalHeader, settings);

            // Assert
            Assert.Equal("NameFirst|Age_Years|City_Location", result);
        }

        [Fact]
        public void CleanHeader_WithSpecialCharacters_ReplacesSpecialCharacters()
        {
            // Arrange
            string originalHeader = "Name#1|Age%Value|City&County";
            var settings = new ProcessingSettings
            {
                ColumnSeparator = "|",
                Replacements = new List<StringReplacement>
                {
                    new StringReplacement { Find = "#", Replace = "_" },
                    new StringReplacement { Find = "%", Replace = "Percent" },
                    new StringReplacement { Find = "&", Replace = "And" }
                }
            };

            // Act
            string result = _processor.CleanHeader(originalHeader, settings);

            // Assert
            Assert.Equal("Name_1|AgePercentValue|CityAndCounty", result);
        }

        [Fact]
        public void CleanHeader_WithDifferentSeparator_WorksWithCommaSeparator()
        {
            // Arrange
            string originalHeader = "Name.First,Age.Years,City.Location";
            var settings = new ProcessingSettings
            {
                ColumnSeparator = ",",
                Replacements = new List<StringReplacement>
                {
                    new StringReplacement { Find = ".", Replace = "_" }
                }
            };

            // Act
            string result = _processor.CleanHeader(originalHeader, settings);

            // Assert
            Assert.Equal("Name_First,Age_Years,City_Location", result);
        }

        [Fact]
        public void CleanHeader_WithEmptyFindString_IgnoresReplacement()
        {
            // Arrange
            string originalHeader = "Name.First|Age.Years";
            var settings = new ProcessingSettings
            {
                ColumnSeparator = "|",
                Replacements = new List<StringReplacement>
                {
                    new StringReplacement { Find = "", Replace = "_" },
                    new StringReplacement { Find = ".", Replace = "" }
                }
            };

            // Act
            string result = _processor.CleanHeader(originalHeader, settings);

            // Assert
            Assert.Equal("NameFirst|AgeYears", result);
        }

        [Fact]
        public void ProcessCsvFileInMemory_WithValidCsvData_ProcessesHeaderOnly()
        {
            // Arrange
            string[] csvLines = new[]
            {
                "Name.First|Age.Years|City.Location",
                "John|25|New York",
                "Jane|30|Los Angeles",
                "Bob|35|Chicago"
            };

            var settings = new ProcessingSettings
            {
                ColumnSeparator = "|",
                Replacements = new List<StringReplacement>
                {
                    new StringReplacement { Find = ".", Replace = "_" }
                }
            };

            // Act
            string[] result = _processor.ProcessCsvFileInMemory(csvLines, settings);

            // Assert
            Assert.Equal(4, result.Length);
            Assert.Equal("Name_First|Age_Years|City_Location", result[0]);
            Assert.Equal("John|25|New York", result[1]);
            Assert.Equal("Jane|30|Los Angeles", result[2]);
            Assert.Equal("Bob|35|Chicago", result[3]);
        }

        [Fact]
        public void ProcessCsvFileInMemory_WithEmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            string[] csvLines = Array.Empty<string>();
            var settings = new ProcessingSettings
            {
                ColumnSeparator = "|",
                Replacements = new List<StringReplacement>
                {
                    new StringReplacement { Find = ".", Replace = "_" }
                }
            };

            // Act
            string[] result = _processor.ProcessCsvFileInMemory(csvLines, settings);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ProcessCsvFileInMemory_WithNullArray_ReturnsEmptyArray()
        {
            // Arrange
            string[] csvLines = null;
            var settings = new ProcessingSettings
            {
                ColumnSeparator = "|",
                Replacements = new List<StringReplacement>
                {
                    new StringReplacement { Find = ".", Replace = "_" }
                }
            };

            // Act
            string[] result = _processor.ProcessCsvFileInMemory(csvLines, settings);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ProcessCsvFile_WithTemporaryFile_ModifiesFileCorrectly()
        {
            // Arrange
            string tempFilePath = Path.GetTempFileName();
            string[] originalContent = new[]
            {
                "Name.First|Age.Years|City.Location",
                "John|25|New York",
                "Jane|30|Los Angeles"
            };

            File.WriteAllLines(tempFilePath, originalContent);

            var settings = new ProcessingSettings
            {
                ColumnSeparator = "|",
                Replacements = new List<StringReplacement>
                {
                    new StringReplacement { Find = ".", Replace = "_" }
                }
            };

            try
            {
                // Act
                _processor.ProcessCsvFile(tempFilePath, settings);

                // Assert
                string[] modifiedContent = File.ReadAllLines(tempFilePath);
                Assert.Equal(3, modifiedContent.Length);
                Assert.Equal("Name_First|Age_Years|City_Location", modifiedContent[0]);
                Assert.Equal("John|25|New York", modifiedContent[1]);
                Assert.Equal("Jane|30|Los Angeles", modifiedContent[2]);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        [Theory]
        [InlineData("Name.First|Age.Years", "|", ".", "", "NameFirst|AgeYears")]
        [InlineData("Name First,Age Years", ",", " ", "_", "Name_First,Age_Years")]
        [InlineData("Name#1;Age%2", ";", "#", "Num", "NameNum1;Age%2")]
        [InlineData("A.B.C|D.E.F", "|", ".", "_", "A_B_C|D_E_F")]
        public void CleanHeader_WithVariousInputs_ProducesExpectedOutput(
            string input, 
            string separator, 
            string find, 
            string replace, 
            string expected)
        {
            // Arrange
            var settings = new ProcessingSettings
            {
                ColumnSeparator = separator,
                Replacements = new List<StringReplacement>
                {
                    new StringReplacement { Find = find, Replace = replace }
                }
            };

            // Act
            string result = _processor.CleanHeader(input, settings);

            // Assert
            Assert.Equal(expected, result);
        }
    }

    public class BasicTests
    {
        [Fact]
        public void ProcessingSettings_CanBeInstantiated()
        {
            // Arrange & Act
            var settings = new ProcessingSettings();

            // Assert
            Assert.NotNull(settings);
            Assert.Equal("*.csv", settings.FilePattern);
            Assert.Equal("|", settings.ColumnSeparator);
            Assert.True(settings.IncludeSubdirectories);
            Assert.Empty(settings.Replacements);
        }

        [Fact]
        public void StringReplacement_CanBeInstantiated()
        {
            // Arrange & Act
            var replacement = new StringReplacement
            {
                Find = ".",
                Replace = "_"
            };

            // Assert
            Assert.Equal(".", replacement.Find);
            Assert.Equal("_", replacement.Replace);
        }
    }
}