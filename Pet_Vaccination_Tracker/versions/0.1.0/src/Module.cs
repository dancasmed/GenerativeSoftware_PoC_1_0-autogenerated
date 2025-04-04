using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PetVaccinationTracker : IGeneratedModule
{
    public string Name { get; set; } = "Pet Vaccination Tracker";

    private string _dataFilePath;
    private List<Pet> _pets;

    public PetVaccinationTracker()
    {
        _pets = new List<Pet>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Pet Vaccination Tracker module is running.");
        _dataFilePath = Path.Combine(dataFolder, "pets_vaccination_records.json");

        LoadData();

        bool running = true;
        while (running)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddPet();
                    break;
                case "2":
                    AddVaccination();
                    break;
                case "3":
                    ScheduleAppointment();
                    break;
                case "4":
                    ViewRecords();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveData();
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nPet Vaccination Tracker");
        Console.WriteLine("1. Add a pet");
        Console.WriteLine("2. Add vaccination record");
        Console.WriteLine("3. Schedule vet appointment");
        Console.WriteLine("4. View all records");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void AddPet()
    {
        Console.Write("Enter pet name: ");
        string name = Console.ReadLine();

        Console.Write("Enter pet type (e.g., Dog, Cat): ");
        string type = Console.ReadLine();

        Console.Write("Enter pet age: ");
        if (int.TryParse(Console.ReadLine(), out int age))
        {
            _pets.Add(new Pet { Name = name, Type = type, Age = age, Vaccinations = new List<Vaccination>(), Appointments = new List<Appointment>() });
            Console.WriteLine("Pet added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid age. Pet not added.");
        }
    }

    private void AddVaccination()
    {
        if (_pets.Count == 0)
        {
            Console.WriteLine("No pets available. Please add a pet first.");
            return;
        }

        DisplayPets();
        Console.Write("Select pet number: ");
        if (int.TryParse(Console.ReadLine(), out int petIndex) && petIndex > 0 && petIndex <= _pets.Count)
        {
            Console.Write("Enter vaccine name: ");
            string vaccineName = Console.ReadLine();

            Console.Write("Enter date administered (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime dateAdministered))
            {
                Console.Write("Enter next due date (yyyy-MM-dd): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime nextDueDate))
                {
                    _pets[petIndex - 1].Vaccinations.Add(new Vaccination
                    {
                        VaccineName = vaccineName,
                        DateAdministered = dateAdministered,
                        NextDueDate = nextDueDate
                    });
                    Console.WriteLine("Vaccination record added successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid next due date. Vaccination not added.");
                }
            }
            else
            {
                Console.WriteLine("Invalid date administered. Vaccination not added.");
            }
        }
        else
        {
            Console.WriteLine("Invalid pet selection.");
        }
    }

    private void ScheduleAppointment()
    {
        if (_pets.Count == 0)
        {
            Console.WriteLine("No pets available. Please add a pet first.");
            return;
        }

        DisplayPets();
        Console.Write("Select pet number: ");
        if (int.TryParse(Console.ReadLine(), out int petIndex) && petIndex > 0 && petIndex <= _pets.Count)
        {
            Console.Write("Enter appointment purpose: ");
            string purpose = Console.ReadLine();

            Console.Write("Enter appointment date (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime appointmentDate))
            {
                Console.Write("Enter vet clinic name: ");
                string clinicName = Console.ReadLine();

                _pets[petIndex - 1].Appointments.Add(new Appointment
                {
                    Purpose = purpose,
                    Date = appointmentDate,
                    ClinicName = clinicName
                });
                Console.WriteLine("Appointment scheduled successfully.");
            }
            else
            {
                Console.WriteLine("Invalid appointment date.");
            }
        }
        else
        {
            Console.WriteLine("Invalid pet selection.");
        }
    }

    private void ViewRecords()
    {
        if (_pets.Count == 0)
        {
            Console.WriteLine("No pets available.");
            return;
        }

        foreach (var pet in _pets)
        {
            Console.WriteLine($"\nPet: {pet.Name} ({pet.Type}), Age: {pet.Age}");
            Console.WriteLine("Vaccinations:");
            foreach (var vaccine in pet.Vaccinations)
            {
                Console.WriteLine($"- {vaccine.VaccineName}, Administered: {vaccine.DateAdministered:yyyy-MM-dd}, Next Due: {vaccine.NextDueDate:yyyy-MM-dd}");
            }

            Console.WriteLine("Appointments:");
            foreach (var appointment in pet.Appointments)
            {
                Console.WriteLine($"- {appointment.Purpose} at {appointment.ClinicName} on {appointment.Date:yyyy-MM-dd}");
            }
        }
    }

    private void DisplayPets()
    {
        Console.WriteLine("\nAvailable Pets:");
        for (int i = 0; i < _pets.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_pets[i].Name} ({_pets[i].Type})");
        }
    }

    private void LoadData()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(_dataFilePath);
                _pets = JsonSerializer.Deserialize<List<Pet>>(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
    }

    private void SaveData()
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(_pets);
            File.WriteAllText(_dataFilePath, jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }
}

public class Pet
{
    public string Name { get; set; }
    public string Type { get; set; }
    public int Age { get; set; }
    public List<Vaccination> Vaccinations { get; set; }
    public List<Appointment> Appointments { get; set; }
}

public class Vaccination
{
    public string VaccineName { get; set; }
    public DateTime DateAdministered { get; set; }
    public DateTime NextDueDate { get; set; }
}

public class Appointment
{
    public string Purpose { get; set; }
    public DateTime Date { get; set; }
    public string ClinicName { get; set; }
}