using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class LandscapingManager : IGeneratedModule
{
    public string Name { get; set; } = "Landscaping Company Manager";

    private string employeesFile;
    private string clientsFile;
    private string jobsFile;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Landscaping Company Manager...");

        employeesFile = Path.Combine(dataFolder, "employees.json");
        clientsFile = Path.Combine(dataFolder, "clients.json");
        jobsFile = Path.Combine(dataFolder, "jobs.json");

        EnsureDataFilesExist();

        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ManageEmployees();
                    break;
                case "2":
                    ManageClients();
                    break;
                case "3":
                    ManageJobs();
                    break;
                case "4":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Landscaping Company Manager is shutting down...");
        return true;
    }

    private void EnsureDataFilesExist()
    {
        if (!File.Exists(employeesFile))
        {
            File.WriteAllText(employeesFile, "[]");
        }

        if (!File.Exists(clientsFile))
        {
            File.WriteAllText(clientsFile, "[]");
        }

        if (!File.Exists(jobsFile))
        {
            File.WriteAllText(jobsFile, "[]");
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nLandscaping Company Manager");
        Console.WriteLine("1. Manage Employees");
        Console.WriteLine("2. Manage Clients");
        Console.WriteLine("3. Manage Jobs");
        Console.WriteLine("4. Exit");
        Console.Write("Enter your choice: ");
    }

    private void ManageEmployees()
    {
        bool backRequested = false;
        while (!backRequested)
        {
            Console.WriteLine("\nEmployee Management");
            Console.WriteLine("1. List Employees");
            Console.WriteLine("2. Add Employee");
            Console.WriteLine("3. Remove Employee");
            Console.WriteLine("4. Back");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ListEmployees();
                    break;
                case "2":
                    AddEmployee();
                    break;
                case "3":
                    RemoveEmployee();
                    break;
                case "4":
                    backRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private void ListEmployees()
    {
        var employees = JsonSerializer.Deserialize<List<Employee>>(File.ReadAllText(employeesFile));
        Console.WriteLine("\nEmployees:");
        foreach (var employee in employees)
        {
            Console.WriteLine("ID: " + employee.Id + ", Name: " + employee.Name + ", Position: " + employee.Position);
        }
    }

    private void AddEmployee()
    {
        Console.Write("Enter employee name: ");
        string name = Console.ReadLine();
        Console.Write("Enter employee position: ");
        string position = Console.ReadLine();

        var employees = JsonSerializer.Deserialize<List<Employee>>(File.ReadAllText(employeesFile));
        employees.Add(new Employee
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Position = position
        });

        File.WriteAllText(employeesFile, JsonSerializer.Serialize(employees));
        Console.WriteLine("Employee added successfully.");
    }

    private void RemoveEmployee()
    {
        ListEmployees();
        Console.Write("Enter employee ID to remove: ");
        string id = Console.ReadLine();

        var employees = JsonSerializer.Deserialize<List<Employee>>(File.ReadAllText(employeesFile));
        var employeeToRemove = employees.Find(e => e.Id == id);

        if (employeeToRemove != null)
        {
            employees.Remove(employeeToRemove);
            File.WriteAllText(employeesFile, JsonSerializer.Serialize(employees));
            Console.WriteLine("Employee removed successfully.");
        }
        else
        {
            Console.WriteLine("Employee not found.");
        }
    }

    private void ManageClients()
    {
        bool backRequested = false;
        while (!backRequested)
        {
            Console.WriteLine("\nClient Management");
            Console.WriteLine("1. List Clients");
            Console.WriteLine("2. Add Client");
            Console.WriteLine("3. Remove Client");
            Console.WriteLine("4. Back");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ListClients();
                    break;
                case "2":
                    AddClient();
                    break;
                case "3":
                    RemoveClient();
                    break;
                case "4":
                    backRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private void ListClients()
    {
        var clients = JsonSerializer.Deserialize<List<Client>>(File.ReadAllText(clientsFile));
        Console.WriteLine("\nClients:");
        foreach (var client in clients)
        {
            Console.WriteLine("ID: " + client.Id + ", Name: " + client.Name + ", Phone: " + client.Phone + ", Email: " + client.Email);
        }
    }

    private void AddClient()
    {
        Console.Write("Enter client name: ");
        string name = Console.ReadLine();
        Console.Write("Enter client phone: ");
        string phone = Console.ReadLine();
        Console.Write("Enter client email: ");
        string email = Console.ReadLine();

        var clients = JsonSerializer.Deserialize<List<Client>>(File.ReadAllText(clientsFile));
        clients.Add(new Client
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Phone = phone,
            Email = email
        });

        File.WriteAllText(clientsFile, JsonSerializer.Serialize(clients));
        Console.WriteLine("Client added successfully.");
    }

    private void RemoveClient()
    {
        ListClients();
        Console.Write("Enter client ID to remove: ");
        string id = Console.ReadLine();

        var clients = JsonSerializer.Deserialize<List<Client>>(File.ReadAllText(clientsFile));
        var clientToRemove = clients.Find(c => c.Id == id);

        if (clientToRemove != null)
        {
            clients.Remove(clientToRemove);
            File.WriteAllText(clientsFile, JsonSerializer.Serialize(clients));
            Console.WriteLine("Client removed successfully.");
        }
        else
        {
            Console.WriteLine("Client not found.");
        }
    }

    private void ManageJobs()
    {
        bool backRequested = false;
        while (!backRequested)
        {
            Console.WriteLine("\nJob Management");
            Console.WriteLine("1. List Jobs");
            Console.WriteLine("2. Add Job");
            Console.WriteLine("3. Remove Job");
            Console.WriteLine("4. Assign Employees to Job");
            Console.WriteLine("5. Back");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ListJobs();
                    break;
                case "2":
                    AddJob();
                    break;
                case "3":
                    RemoveJob();
                    break;
                case "4":
                    AssignEmployeesToJob();
                    break;
                case "5":
                    backRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private void ListJobs()
    {
        var jobs = JsonSerializer.Deserialize<List<Job>>(File.ReadAllText(jobsFile));
        Console.WriteLine("\nJobs:");
        foreach (var job in jobs)
        {
            Console.WriteLine("ID: " + job.Id + ", Description: " + job.Description + ", Client: " + job.ClientId + ", Address: " + job.Address);
            Console.WriteLine("Start: " + job.StartDate + ", End: " + job.EndDate + ", Duration: " + job.ExpectedDuration);
            Console.WriteLine("Price: " + job.SellingPrice + ", Cost: " + job.RealCost);
            Console.WriteLine("Assigned Employees: " + string.Join(", ", job.AssignedEmployeeIds));
        }
    }

    private void AddJob()
    {
        Console.Write("Enter job description: ");
        string description = Console.ReadLine();
        Console.Write("Enter client ID: ");
        string clientId = Console.ReadLine();
        Console.Write("Enter address: ");
        string address = Console.ReadLine();
        Console.Write("Enter start date (yyyy-MM-dd): ");
        DateTime startDate = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter end date (yyyy-MM-dd): ");
        DateTime endDate = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter expected duration (hours): ");
        double expectedDuration = double.Parse(Console.ReadLine());
        Console.Write("Enter selling price: ");
        decimal sellingPrice = decimal.Parse(Console.ReadLine());
        Console.Write("Enter real cost: ");
        decimal realCost = decimal.Parse(Console.ReadLine());

        var jobs = JsonSerializer.Deserialize<List<Job>>(File.ReadAllText(jobsFile));
        jobs.Add(new Job
        {
            Id = Guid.NewGuid().ToString(),
            Description = description,
            ClientId = clientId,
            Address = address,
            StartDate = startDate,
            EndDate = endDate,
            ExpectedDuration = expectedDuration,
            SellingPrice = sellingPrice,
            RealCost = realCost,
            AssignedEmployeeIds = new List<string>()
        });

        File.WriteAllText(jobsFile, JsonSerializer.Serialize(jobs));
        Console.WriteLine("Job added successfully.");
    }

    private void RemoveJob()
    {
        ListJobs();
        Console.Write("Enter job ID to remove: ");
        string id = Console.ReadLine();

        var jobs = JsonSerializer.Deserialize<List<Job>>(File.ReadAllText(jobsFile));
        var jobToRemove = jobs.Find(j => j.Id == id);

        if (jobToRemove != null)
        {
            jobs.Remove(jobToRemove);
            File.WriteAllText(jobsFile, JsonSerializer.Serialize(jobs));
            Console.WriteLine("Job removed successfully.");
        }
        else
        {
            Console.WriteLine("Job not found.");
        }
    }

    private void AssignEmployeesToJob()
    {
        ListJobs();
        Console.Write("Enter job ID to assign employees: ");
        string jobId = Console.ReadLine();

        var jobs = JsonSerializer.Deserialize<List<Job>>(File.ReadAllText(jobsFile));
        var job = jobs.Find(j => j.Id == jobId);

        if (job == null)
        {
            Console.WriteLine("Job not found.");
            return;
        }

        ListEmployees();
        Console.Write("Enter employee IDs to assign (comma separated): ");
        string[] employeeIds = Console.ReadLine().Split(',');

        job.AssignedEmployeeIds = new List<string>(employeeIds);
        File.WriteAllText(jobsFile, JsonSerializer.Serialize(jobs));
        Console.WriteLine("Employees assigned successfully.");
    }
}

public class Employee
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
}

public class Client
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}

public class Job
{
    public string Id { get; set; }
    public string Description { get; set; }
    public string ClientId { get; set; }
    public string Address { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double ExpectedDuration { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal RealCost { get; set; }
    public List<string> AssignedEmployeeIds { get; set; }
}