using System;
using System.IO;
using System.Linq;

namespace DeDottifyHeader
{
    public class CsvHeaderProcessor
    {
        public string CleanHeader(string header, ProcessingSettings settings)
        {
            // If no replacements configured, return original header
            if (settings.Replacements == null || !settings.Replacements.Any())
            {
                return header;
            }

            // Split by the configured separator, apply replacements to each column name, then rejoin
            string[] columns = header.Split(settings.ColumnSeparator);
            
            for (int i = 0; i < columns.Length; i++)
            {
                string originalColumn = columns[i];
                string cleanedColumn = originalColumn;

                // Apply all configured replacements to each column
                foreach (var replacement in settings.Replacements)
                {
                    if (!string.IsNullOrEmpty(replacement.Find))
                    {
                        cleanedColumn = cleanedColumn.Replace(replacement.Find, replacement.Replace ?? string.Empty);
                    }
                }

                columns[i] = cleanedColumn;
            }
            
            return string.Join(settings.ColumnSeparator, columns);
        }

        public void ProcessCsvFile(string filePath, ProcessingSettings settings)
        {
            Console.WriteLine($"Processing: {filePath}");

            // Read all lines from the file
            string[] lines = File.ReadAllLines(filePath);
            
            if (lines.Length == 0)
            {
                Console.WriteLine($"File is empty: {filePath}");
                return;
            }

            // Process the header (first line)
            string originalHeader = lines[0];
            string cleanedHeader = CleanHeader(originalHeader, settings);

            if (originalHeader != cleanedHeader)
            {
                // Replace the header with the cleaned version
                lines[0] = cleanedHeader;
                
                // Write the modified content back to the file
                File.WriteAllLines(filePath, lines);
                
                Console.WriteLine($"Header updated in: {filePath}");
                Console.WriteLine($"  Original: {originalHeader}");
                Console.WriteLine($"  Cleaned:  {cleanedHeader}");
            }
            else
            {
                Console.WriteLine($"No changes needed for: {filePath}");
            }
        }

        public string[] ProcessCsvFileInMemory(string[] csvLines, ProcessingSettings settings)
        {
            if (csvLines == null || csvLines.Length == 0)
            {
                return csvLines ?? Array.Empty<string>();
            }

            // Create a copy to avoid modifying the original array
            string[] processedLines = new string[csvLines.Length];
            Array.Copy(csvLines, processedLines, csvLines.Length);

            // Process the header (first line)
            string originalHeader = processedLines[0];
            string cleanedHeader = CleanHeader(originalHeader, settings);
            processedLines[0] = cleanedHeader;

            return processedLines;
        }
    }
}