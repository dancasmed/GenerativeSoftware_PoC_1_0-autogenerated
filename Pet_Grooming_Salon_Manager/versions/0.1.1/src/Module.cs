using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class PetGroomingSalonModule : IGeneratedModule
{
    public string Name { get; set; } = "Pet Grooming Salon Manager";

    private string _appointmentsFilePath;
    private string _servicesFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Pet Grooming Salon Manager...");

        _appointmentsFilePath = Path.Combine(dataFolder, "appointments.json");
        _servicesFilePath = Path.Combine(dataFolder, "services.json");

        InitializeDataFiles();

        bool running = true;
        while (running)
        {
            DisplayMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddAppointment();
                    break;
                case "2":
                    ViewAppointments();
                    break;
                case "3":
                    AddService();
                    break;
                case "4":
                    ViewServices();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Pet Grooming Salon Manager is shutting down...");
        return true;
    }

    private void InitializeDataFiles()
    {
        if (!File.Exists(_appointmentsFilePath))
        {
            File.WriteAllText(_appointmentsFilePath, "[]");
        }

        if (!File.Exists(_servicesFilePath))
        {
            var defaultServices = new List<Service>
            {
                new Service { Id = 1, Name = "Basic Bath", Price = 25.00m, Duration = 30 },
                new Service { Id = 2, Name = "Full Grooming", Price = 50.00m, Duration = 60 },
                new Service { Id = 3, Name = "Nail Trimming", Price = 15.00m, Duration = 15 }
            };
            File.WriteAllText(_servicesFilePath, JsonSerializer.Serialize(defaultServices));
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nPet Grooming Salon Manager");
        Console.WriteLine("1. Add Appointment");
        Console.WriteLine("2. View Appointments");
        Console.WriteLine("3. Add Service");
        Console.WriteLine("4. View Services");
        Console.WriteLine("5. Exit");
        Console.Write("Enter your choice: ");
    }

    private void AddAppointment()
    {
        Console.WriteLine("\nAdd New Appointment");

        try
        {
            var appointments = GetAppointments();
            var services = GetServices();

            if (services.Count == 0)
            {
                Console.WriteLine("No services available. Please add services first.");
                return;
            }

            Console.WriteLine("Available Services:");
            foreach (var service in services)
            {
                Console.WriteLine(service.Id + ". " + service.Name + " (" + service.Price + ")");
            }

            Console.Write("Enter pet name: ");
            var petName = Console.ReadLine();

            Console.Write("Enter owner name: ");
            var ownerName = Console.ReadLine();

            Console.Write("Enter service ID: ");
            if (!int.TryParse(Console.ReadLine(), out int serviceId))
            {
                Console.WriteLine("Invalid service ID.");
                return;
            }

            var selectedService = services.Find(s => s.Id == serviceId);
            if (selectedService == null)
            {
                Console.WriteLine("Service not found.");
                return;
            }

            Console.Write("Enter appointment date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.Write("Enter appointment time (HH:mm): ");
            if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan time))
            {
                Console.WriteLine("Invalid time format.");
                return;
            }

            var appointmentDateTime = date.Add(time);

            var newAppointment = new Appointment
            {
                Id = appointments.Count > 0 ? appointments.Max(a => a.Id) + 1 : 1,
                PetName = petName,
                OwnerName = ownerName,
                ServiceId = serviceId,
                AppointmentDateTime = appointmentDateTime,
                IsCompleted = false
            };

            appointments.Add(newAppointment);
            SaveAppointments(appointments);

            Console.WriteLine("Appointment added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding appointment: " + ex.Message);
        }
    }

    private void ViewAppointments()
    {
        Console.WriteLine("\nAppointment List");

        var appointments = GetAppointments();
        var services = GetServices();

        if (appointments.Count == 0)
        {
            Console.WriteLine("No appointments found.");
            return;
        }

        foreach (var appointment in appointments)
        {
            var service = services.Find(s => s.Id == appointment.ServiceId);
            Console.WriteLine("ID: " + appointment.Id);
            Console.WriteLine("Pet: " + appointment.PetName);
            Console.WriteLine("Owner: " + appointment.OwnerName);
            Console.WriteLine("Service: " + (service?.Name ?? "Unknown"));
            Console.WriteLine("Date/Time: " + appointment.AppointmentDateTime);
            Console.WriteLine("Status: " + (appointment.IsCompleted ? "Completed" : "Pending"));
            Console.WriteLine("-----------------------");
        }
    }

    private void AddService()
    {
        Console.WriteLine("\nAdd New Service");

        try
        {
            var services = GetServices();

            Console.Write("Enter service name: ");
            var name = Console.ReadLine();

            Console.Write("Enter service price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price.");
                return;
            }

            Console.Write("Enter service duration in minutes: ");
            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                Console.WriteLine("Invalid duration.");
                return;
            }

            var newService = new Service
            {
                Id = services.Count > 0 ? services.Max(s => s.Id) + 1 : 1,
                Name = name,
                Price = price,
                Duration = duration
            };

            services.Add(newService);
            SaveServices(services);

            Console.WriteLine("Service added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding service: " + ex.Message);
        }
    }

    private void ViewServices()
    {
        Console.WriteLine("\nService List");

        var services = GetServices();

        if (services.Count == 0)
        {
            Console.WriteLine("No services found.");
            return;
        }

        foreach (var service in services)
        {
            Console.WriteLine("ID: " + service.Id);
            Console.WriteLine("Name: " + service.Name);
            Console.WriteLine("Price: " + service.Price);
            Console.WriteLine("Duration: " + service.Duration + " minutes");
            Console.WriteLine("-----------------------");
        }
    }

    private List<Appointment> GetAppointments()
    {
        var json = File.ReadAllText(_appointmentsFilePath);
        return JsonSerializer.Deserialize<List<Appointment>>(json) ?? new List<Appointment>();
    }

    private void SaveAppointments(List<Appointment> appointments)
    {
        var json = JsonSerializer.Serialize(appointments);
        File.WriteAllText(_appointmentsFilePath, json);
    }

    private List<Service> GetServices()
    {
        var json = File.ReadAllText(_servicesFilePath);
        return JsonSerializer.Deserialize<List<Service>>(json) ?? new List<Service>();
    }

    private void SaveServices(List<Service> services)
    {
        var json = JsonSerializer.Serialize(services);
        File.WriteAllText(_servicesFilePath, json);
    }
}

public class Appointment
{
    public int Id { get; set; }
    public string PetName { get; set; }
    public string OwnerName { get; set; }
    public int ServiceId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public bool IsCompleted { get; set; }
}

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
}