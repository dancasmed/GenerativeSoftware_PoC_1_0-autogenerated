using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class FreelanceClientManager : IGeneratedModule
{
    public string Name { get; set; } = "Freelance Client Manager";
    
    private string _clientsFilePath;
    private string _paymentsFilePath;
    
    public FreelanceClientManager()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Freelance Client Manager module is running.");
        
        _clientsFilePath = Path.Combine(dataFolder, "clients.json");
        _paymentsFilePath = Path.Combine(dataFolder, "payments.json");
        
        EnsureDataFilesExist();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddClient();
                    break;
                case "2":
                    ListClients();
                    break;
                case "3":
                    RecordPayment();
                    break;
                case "4":
                    ViewPayments();
                    break;
                case "5":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Freelance Client Manager module completed.");
        return true;
    }
    
    private void EnsureDataFilesExist()
    {
        if (!File.Exists(_clientsFilePath))
        {
            File.WriteAllText(_clientsFilePath, "[]");
        }
        
        if (!File.Exists(_paymentsFilePath))
        {
            File.WriteAllText(_paymentsFilePath, "[]");
        }
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nFreelance Client Manager");
        Console.WriteLine("1. Add New Client");
        Console.WriteLine("2. List All Clients");
        Console.WriteLine("3. Record Payment");
        Console.WriteLine("4. View Payments");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddClient()
    {
        Console.Write("Enter client name: ");
        var name = Console.ReadLine();
        
        Console.Write("Enter client email: ");
        var email = Console.ReadLine();
        
        Console.Write("Enter hourly rate: ");
        var rateInput = Console.ReadLine();
        
        if (!decimal.TryParse(rateInput, out var hourlyRate))
        {
            Console.WriteLine("Invalid hourly rate. Client not added.");
            return;
        }
        
        var clients = LoadClients();
        clients.Add(new Client
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            HourlyRate = hourlyRate,
            CreatedDate = DateTime.Now
        });
        
        SaveClients(clients);
        Console.WriteLine("Client added successfully.");
    }
    
    private void ListClients()
    {
        var clients = LoadClients();
        
        if (clients.Count == 0)
        {
            Console.WriteLine("No clients found.");
            return;
        }
        
        Console.WriteLine("\nClient List:");
        foreach (var client in clients)
        {
            Console.WriteLine("ID: " + client.Id);
            Console.WriteLine("Name: " + client.Name);
            Console.WriteLine("Email: " + client.Email);
            Console.WriteLine("Hourly Rate: " + client.HourlyRate);
            Console.WriteLine("Created: " + client.CreatedDate);
            Console.WriteLine("--------------------");
        }
    }
    
    private void RecordPayment()
    {
        var clients = LoadClients();
        if (clients.Count == 0)
        {
            Console.WriteLine("No clients available. Please add a client first.");
            return;
        }
        
        ListClients();
        Console.Write("Enter client ID: ");
        var clientIdInput = Console.ReadLine();
        
        if (!Guid.TryParse(clientIdInput, out var clientId))
        {
            Console.WriteLine("Invalid client ID format.");
            return;
        }
        
        var client = clients.Find(c => c.Id == clientId);
        if (client == null)
        {
            Console.WriteLine("Client not found.");
            return;
        }
        
        Console.Write("Enter payment amount: ");
        var amountInput = Console.ReadLine();
        
        if (!decimal.TryParse(amountInput, out var amount))
        {
            Console.WriteLine("Invalid payment amount.");
            return;
        }
        
        Console.Write("Enter payment description: ");
        var description = Console.ReadLine();
        
        var payments = LoadPayments();
        payments.Add(new Payment
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            Amount = amount,
            Description = description,
            PaymentDate = DateTime.Now
        });
        
        SavePayments(payments);
        Console.WriteLine("Payment recorded successfully.");
    }
    
    private void ViewPayments()
    {
        var payments = LoadPayments();
        var clients = LoadClients();
        
        if (payments.Count == 0)
        {
            Console.WriteLine("No payments found.");
            return;
        }
        
        Console.WriteLine("\nPayment History:");
        foreach (var payment in payments)
        {
            var client = clients.Find(c => c.Id == payment.ClientId);
            var clientName = client != null ? client.Name : "[Deleted Client]";
            
            Console.WriteLine("ID: " + payment.Id);
            Console.WriteLine("Client: " + clientName);
            Console.WriteLine("Amount: " + payment.Amount);
            Console.WriteLine("Description: " + payment.Description);
            Console.WriteLine("Date: " + payment.PaymentDate);
            Console.WriteLine("--------------------");
        }
    }
    
    private List<Client> LoadClients()
    {
        var json = File.ReadAllText(_clientsFilePath);
        return JsonSerializer.Deserialize<List<Client>>(json) ?? new List<Client>();
    }
    
    private void SaveClients(List<Client> clients)
    {
        var json = JsonSerializer.Serialize(clients);
        File.WriteAllText(_clientsFilePath, json);
    }
    
    private List<Payment> LoadPayments()
    {
        var json = File.ReadAllText(_paymentsFilePath);
        return JsonSerializer.Deserialize<List<Payment>>(json) ?? new List<Payment>();
    }
    
    private void SavePayments(List<Payment> payments)
    {
        var json = JsonSerializer.Serialize(payments);
        File.WriteAllText(_paymentsFilePath, json);
    }
}

public class Client
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class Payment
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime PaymentDate { get; set; }
}