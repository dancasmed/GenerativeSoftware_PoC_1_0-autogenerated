using SelfEvolvingSoftware.Interfaces;
using System.Text.Json;
using System.IO;
using System;
using System.Collections.Generic;

public class VinylManagerModule : IGeneratedModule {
    public string Name { get; set; } = "Vinyl Collection Manager";
    
    private string _recordsPath;
    private string _wishlistPath;
    private string _reportsPath;
    
    public bool Main(string dataFolder) {
        Console.WriteLine("Initializing Vinyl Collection Manager...");
        
        _recordsPath = Path.Combine(dataFolder, "records.json");
        _wishlistPath = Path.Combine(dataFolder, "wishlist.json");
        _reportsPath = Path.Combine(dataFolder, "reports.json");
        
        InitializeFiles();
        
        while (ShowMainMenu()) { }
        
        Console.WriteLine("Exiting Vinyl Collection Manager");
        return true;
    }
    
    private void InitializeFiles() {
        foreach (var path in new[] { _recordsPath, _wishlistPath, _reportsPath }) {
            if (!File.Exists(path)) {
                File.WriteAllText(path, "[]");
            }
        }
    }
    
    private bool ShowMainMenu() {
        Console.WriteLine("\nMain Menu:");
        Console.WriteLine("1. Manage Records");
        Console.WriteLine("2. Manage Wishlist");
        Console.WriteLine("3. Generate Reports");
        Console.WriteLine("4. Exit");
        
        switch (Console.ReadLine()) {
            case "1":
                ManageRecords();
                return true;
            case "2":
                ManageWishlist();
                return true;
            case "3":
                GenerateReportMenu();
                return true;
            case "4":
                return false;
            default:
                Console.WriteLine("Invalid option");
                return true;
        }
    }
    
    private void ManageRecords() {
        Console.WriteLine("\nRecord Management:");
        Console.WriteLine("1. Add Record");
        Console.WriteLine("2. Edit Record");
        Console.WriteLine("3. Delete Record");
        Console.WriteLine("4. List All Records");
        Console.WriteLine("5. Search Records");
        Console.WriteLine("6. Back");
        
        var records = LoadRecords();
        
        switch (Console.ReadLine()) {
            case "1":
                AddRecord(records);
                break;
            case "2":
                EditRecord(records);
                break;
            case "3":
                DeleteRecord(records);
                break;
            case "4":
                ListRecords(records);
                break;
            case "5":
                SearchRecords(records);
                break;
        }
    }
    
    private List<Record> LoadRecords() {
        return JsonSerializer.Deserialize<List<Record>>(File.ReadAllText(_recordsPath));
    }
    
    private void SaveRecords(List<Record> records) {
        File.WriteAllText(_recordsPath, JsonSerializer.Serialize(records));
    }
    
    private void AddRecord(List<Record> records) {
        var newRecord = new Record {
            Id = Guid.NewGuid(),
            Title = ReadInput("Title"),
            Artist = ReadInput("Artist"),
            Year = int.Parse(ReadInput("Year")),
            Genre = ReadInput("Genre"),
            Condition = ReadInput("Condition (Mint/Good/Fair)"),
            Value = decimal.Parse(ReadInput("Value"))
        };
        
        records.Add(newRecord);
        SaveRecords(records);
        Console.WriteLine("Record added successfully");
    }
    
    private string ReadInput(string prompt) {
        Console.Write(prompt + ": ");
        return Console.ReadLine();
    }
    
    private void GenerateReportMenu() {
        Console.WriteLine("\nReport Types:");
        Console.WriteLine("1. Collection Summary");
        Console.WriteLine("2. Genre Distribution");
        Console.WriteLine("3. Value Report");
        
        var records = LoadRecords();
        var report = new Report {
            Id = Guid.NewGuid(),
            Date = DateTime.Now
        };
        
        switch (Console.ReadLine()) {
            case "1":
                report.Type = "Summary";
                report.Content = "Total Records: " + records.Count;
                break;
            case "2":
                report.Type = "Genre Distribution";
                report.Content = string.Join("\n", 
                    records.GroupBy(r => r.Genre)
                           .Select(g => g.Key + ": " + g.Count()));
                break;
            case "3":
                report.Type = "Value Report";
                report.Content = "Total Collection Value: " + 
                    records.Sum(r => r.Value).ToString("C");
                break;
        }
        
        SaveReport(report);
        Console.WriteLine("Report generated: \n" + report.Content);
    }
    
    private void SaveReport(Report report) {
        var reports = JsonSerializer.Deserialize<List<Report>>(File.ReadAllText(_reportsPath));
        reports.Add(report);
        File.WriteAllText(_reportsPath, JsonSerializer.Serialize(reports));
    }
    
    private void ManageWishlist() {
        Console.WriteLine("Manage Wishlist functionality not implemented.");
    }
    
    private void EditRecord(List<Record> records) {
        Console.WriteLine("Edit Record functionality not implemented.");
    }
    
    private void DeleteRecord(List<Record> records) {
        Console.WriteLine("Delete Record functionality not implemented.");
    }
    
    private void ListRecords(List<Record> records) {
        Console.WriteLine("List Records functionality not implemented.");
    }
    
    private void SearchRecords(List<Record> records) {
        Console.WriteLine("Search Records functionality not implemented.");
    }
}

public class Record {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public int Year { get; set; }
    public string Genre { get; set; }
    public string Condition { get; set; }
    public decimal Value { get; set; }
}

public class Wishlist {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public int Year { get; set; }
    public string Genre { get; set; }
    public string Priority { get; set; }
}

public class Report {
    public Guid Id { get; set; }
    public string Type { get; set; }
    public DateTime Date { get; set; }
    public string Content { get; set; }
}