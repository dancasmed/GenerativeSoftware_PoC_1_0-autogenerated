using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

public class ExpenseTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Expense Tracker";
    private string _dataFolder;
    private User _currentUser;
    private DataService _dataService;
    private List<User> _users;
    private List<Category> _categories;
    private List<Expense> _expenses;

    public bool Main(string dataFolder)
    {
        _dataFolder = dataFolder;
        _dataService = new DataService(dataFolder);
        Console.WriteLine("Initializing Expense Tracker Module...");

        LoadData();

        if (!HandleAuthentication())
            return false;

        while (true)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Add Expense");
            Console.WriteLine("2. View/Edit Expenses");
            Console.WriteLine("3. Generate Report");
            Console.WriteLine("4. Exit");
            Console.Write("Select option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    AddExpense();
                    break;
                case "2":
                    ViewEditExpenses();
                    break;
                case "3":
                    GenerateMonthlyReport();
                    break;
                case "4":
                    return true;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    private void LoadData()
    {
        _users = _dataService.LoadData<User>("users.json");
        _categories = _dataService.LoadData<Category>("categories.json");
        _expenses = _dataService.LoadData<Expense>("expenses.json");

        if (!_categories.Any(c => c.UserId == Guid.Empty))
        {
            _categories.AddRange(new[]
            {
                new Category { Id = Guid.NewGuid(), Name = "Food", UserId = Guid.Empty },
                new Category { Id = Guid.NewGuid(), Name = "Transport", UserId = Guid.Empty },
                new Category { Id = Guid.NewGuid(), Name = "Housing", UserId = Guid.Empty }
            });
            _dataService.SaveData("categories.json", _categories);
        }
    }

    private bool HandleAuthentication()
    {
        while (true)
        {
            Console.WriteLine("\n1. Login\n2. Register\n3. Exit");
            string choice = Console.ReadLine();

            if (choice == "3") return false;

            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            if (choice == "1")
            {
                var user = _users.FirstOrDefault(u => u.Username == username);
                if (user != null && SecurityHelper.VerifyPassword(password, user.PasswordHash))
                {
                    _currentUser = user;
                    return true;
                }
                Console.WriteLine("Invalid credentials");
            }
            else if (choice == "2")
            {
                if (_users.Any(u => u.Username == username))
                {
                    Console.WriteLine("Username exists");
                    continue;
                }

                _currentUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    PasswordHash = SecurityHelper.HashPassword(password)
                };
                _users.Add(_currentUser);
                _dataService.SaveData("users.json", _users);
                return true;
            }
        }
    }

    private void AddExpense()
    {
        Console.Write("Amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount");
            return;
        }

        Console.Write("Date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.WriteLine("Invalid date");
            return;
        }

        Console.WriteLine("Available categories: " + string.Join(", ", _categories.Select(c => c.Name)));
        Console.Write("Category name: ");
        string categoryName = Console.ReadLine();

        var category = _categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        if (category == null)
        {
            category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryName,
                UserId = _currentUser.Id
            };
            _categories.Add(category);
            _dataService.SaveData("categories.json", _categories);
        }

        Console.Write("Notes (optional): ");
        string notes = Console.ReadLine();

        _expenses.Add(new Expense
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Date = date,
            CategoryId = category.Id,
            UserId = _currentUser.Id,
            Notes = notes
        });

        _dataService.SaveData("expenses.json", _expenses);
        Console.WriteLine("Expense added successfully");
    }

    private void ViewEditExpenses()
    {
        var userExpenses = _expenses.Where(e => e.UserId == _currentUser.Id).ToList();
        if (!userExpenses.Any())
        {
            Console.WriteLine("No expenses found");
            return;
        }

        for (int i = 0; i < userExpenses.Count; i++)
        {
            var expense = userExpenses[i];
            var category = _categories.First(c => c.Id == expense.CategoryId);
            Console.WriteLine(string.Format("{0}. {1} - {2} ({3}) {4}",
                i + 1,
                expense.Date.ToString("yyyy-MM-dd"),
                expense.Amount,
                category.Name,
                expense.Notes));
        }

        Console.Write("Select expense to edit/delete (0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int selection) || selection == 0)
            return;

        if (selection < 1 || selection > userExpenses.Count)
        {
            Console.WriteLine("Invalid selection");
            return;
        }

        var selectedExpense = userExpenses[selection - 1];
        Console.WriteLine("1. Edit\n2. Delete");
        string action = Console.ReadLine();

        if (action == "1")
        {
            Console.Write("New amount (current: " + selectedExpense.Amount + "): ");
            if (decimal.TryParse(Console.ReadLine(), out decimal newAmount))
                selectedExpense.Amount = newAmount;

            Console.Write("New date (yyyy-MM-dd) (current: " + selectedExpense.Date.ToString("yyyy-MM-dd") + "): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime newDate))
                selectedExpense.Date = newDate;

            Console.Write("New notes (current: " + selectedExpense.Notes + "): ");
            selectedExpense.Notes = Console.ReadLine();

            _dataService.SaveData("expenses.json", _expenses);
            Console.WriteLine("Expense updated");
        }
        else if (action == "2")
        {
            _expenses.Remove(selectedExpense);
            _dataService.SaveData("expenses.json", _expenses);
            Console.WriteLine("Expense deleted");
        }
    }

    private void GenerateMonthlyReport()
    {
        Console.Write("Month (1-12): ");
        if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
        {
            Console.WriteLine("Invalid month");
            return;
        }

        Console.Write("Year: ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year");
            return;
        }

        var reportData = _expenses
            .Where(e => e.UserId == _currentUser.Id && e.Date.Month == month && e.Date.Year == year)
            .GroupBy(e => e.CategoryId)
            .Select(g => new
            {
                Category = _categories.First(c => c.Id == g.Key).Name,
                Total = g.Sum(e => e.Amount)
            }).ToList();

        Console.WriteLine("\nMonthly Report:");
        foreach (var item in reportData)
        {
            Console.WriteLine(string.Format("{0}: {1:C}", item.Category, item.Total));
        }
    }
}

public class DataService
{
    private readonly string _dataFolder;

    public DataService(string dataFolder)
    {
        _dataFolder = dataFolder;
        Directory.CreateDirectory(dataFolder);
    }

    public List<T> LoadData<T>(string fileName)
    {
        string path = Path.Combine(_dataFolder, fileName);
        if (!File.Exists(path)) return new List<T>();

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public void SaveData<T>(string fileName, List<T> data)
    {
        string path = Path.Combine(_dataFolder, fileName);
        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(path, json);
    }
}

public static class SecurityHelper
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        return HashPassword(password) == storedHash;
    }
}

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid UserId { get; set; }
}

public class Expense
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
    public string Notes { get; set; }
    public Guid UserId { get; set; }
}
