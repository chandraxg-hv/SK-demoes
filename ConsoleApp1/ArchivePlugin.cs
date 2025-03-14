using System;
using System.ComponentModel;
using Microsoft.SemanticKernel; // Replace with the actual namespace where KernelFunction is defined

namespace ConsoleApp1;
public class ArchivePlugin
{
    [KernelFunction("archive_data")]
    [Description("Save data to a specified file in the compute ")]
    //write a async task method to archive data into a file
    public async Task ArchiveDataAsync(Kernel kernel, string data, string filename = "newsarchive.txt")
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException("Data cannot be null or empty", nameof(data));
        }

        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filename));
        }

        try
        {
            using (StreamWriter writer = new StreamWriter($"/Users/cganapathy/Demoes/sk-demoes/ConsoleApp1/{filename}", append: true))
            {
                await writer.WriteLineAsync(data);
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log the error)
            Console.WriteLine($"An error occurred while archiving data: {ex.Message}");
        }
    }
}

