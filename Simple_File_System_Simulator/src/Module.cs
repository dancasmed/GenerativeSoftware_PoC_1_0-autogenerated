using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SimpleFileSystem : IGeneratedModule
{
    public string Name { get; set; } = "Simple File System Simulator";
    private string _dataFilePath;
    private List<string> _files;

    public SimpleFileSystem()
    {
        _files = new List<string>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Simple File System Simulator...");
        _dataFilePath = Path.Combine(dataFolder, "filesystem.json");
        
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(_dataFilePath);
                _files = JsonSerializer.Deserialize<List<string>>(jsonData);
                Console.WriteLine("File system data loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading file system data: " + ex.Message);
                return false;
            }
        }
        else
        {
            Console.WriteLine("No existing file system data found. Starting fresh.");
        }

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Create a file");
            Console.WriteLine("2. Delete a file");
            Console.WriteLine("3. List all files");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    CreateFile();
                    break;
                case "2":
                    DeleteFile();
                    break;
                case "3":
                    ListFiles();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        try
        {
            string jsonData = JsonSerializer.Serialize(_files);
            File.WriteAllText(_dataFilePath, jsonData);
            Console.WriteLine("File system data saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving file system data: " + ex.Message);
            return false;
        }
    }

    private void CreateFile()
    {
        Console.Write("Enter file name to create: ");
        string fileName = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Console.WriteLine("File name cannot be empty.");
            return;
        }
        
        if (_files.Contains(fileName))
        {
            Console.WriteLine("File already exists.");
        }
        else
        {
            _files.Add(fileName);
            Console.WriteLine("File created successfully.");
        }
    }

    private void DeleteFile()
    {
        if (_files.Count == 0)
        {
            Console.WriteLine("No files to delete.");
            return;
        }
        
        Console.Write("Enter file name to delete: ");
        string fileName = Console.ReadLine();
        
        if (_files.Remove(fileName))
        {
            Console.WriteLine("File deleted successfully.");
        }
        else
        {
            Console.WriteLine("File not found.");
        }
    }

    private void ListFiles()
    {
        if (_files.Count == 0)
        {
            Console.WriteLine("No files exist.");
            return;
        }
        
        Console.WriteLine("Files:");
        foreach (string file in _files)
        {
            Console.WriteLine(file);
        }
    }
}