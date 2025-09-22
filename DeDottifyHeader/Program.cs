using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace DeDottifyHeader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DeDottify Header Tool - Starting...");

            try
            {
                // Load configuration from appsettings.json
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var appSettings = new AppSettings();
                configuration.Bind(appSettings);

                // Validate configuration
                if (string.IsNullOrWhiteSpace(appSettings.ProcessingSettings.TargetDirectory))
                {
                    Console.WriteLine("Error: TargetDirectory is not configured in appsettings.json");
                    return;
                }

                Console.WriteLine($"Configuration loaded:");
                Console.WriteLine($"  Target Directory: {appSettings.ProcessingSettings.TargetDirectory}");
                Console.WriteLine($"  File Pattern: {appSettings.ProcessingSettings.FilePattern}");
                Console.WriteLine($"  Include Subdirectories: {appSettings.ProcessingSettings.IncludeSubdirectories}");
                Console.WriteLine($"  Column Separator: '{appSettings.ProcessingSettings.ColumnSeparator}'");
                Console.WriteLine($"  Replacements configured: {appSettings.ProcessingSettings.Replacements.Count}");
                
                if (appSettings.ProcessingSettings.Replacements.Any())
                {
                    Console.WriteLine("  Replacement rules:");
                    foreach (var replacement in appSettings.ProcessingSettings.Replacements)
                    {
                        Console.WriteLine($"    '{replacement.Find}' → '{replacement.Replace}'");
                    }
                }
                else
                {
                    Console.WriteLine("  Warning: No replacement rules configured. Headers will not be modified.");
                }
                Console.WriteLine();

                var processor = new CsvHeaderProcessor();
                ProcessCsvFiles(appSettings.ProcessingSettings, processor);
                Console.WriteLine("Processing completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

        static void ProcessCsvFiles(ProcessingSettings settings, CsvHeaderProcessor processor)
        {
            if (!Directory.Exists(settings.TargetDirectory))
            {
                Console.WriteLine($"Directory does not exist: {settings.TargetDirectory}");
                return;
            }

            // Get all CSV files in the directory and subdirectories
            SearchOption searchOption = settings.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] csvFiles = Directory.GetFiles(settings.TargetDirectory, settings.FilePattern, searchOption);
            
            Console.WriteLine($"Found {csvFiles.Length} CSV files to process...");

            foreach (string csvFile in csvFiles)
            {
                try
                {
                    processor.ProcessCsvFile(csvFile, settings);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {csvFile}: {ex.Message}");
                }
            }
        }
    }
}
