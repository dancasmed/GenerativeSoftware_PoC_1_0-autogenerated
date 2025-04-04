using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class CarRentalModule
{
    public string Name { get; set; } = "Car Rental Inventory Manager";
    
    private string _dataFilePath;
    
    public CarRentalModule()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Car Rental Inventory Manager...");
        
        _dataFilePath = Path.Combine(dataFolder, "carInventory.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        if (!File.Exists(_dataFilePath))
        {
            InitializeDefaultInventory();
            Console.WriteLine("Created new inventory file with default data.");
        }
        
        DisplayMainMenu();
        
        return true;
    }
    
    private void InitializeDefaultInventory()
    {
        var defaultCars = new List<Car>
        {
            new Car { Id = 1, Make = "Toyota", Model = "Camry", Year = 2022, DailyRate = 45.99m, IsAvailable = true },
            new Car { Id = 2, Make = "Honda", Model = "Civic", Year = 2021, DailyRate = 39.99m, IsAvailable = true },
            new Car { Id = 3, Make = "Ford", Model = "Mustang", Year = 2023, DailyRate = 69.99m, IsAvailable = true }
        };
        
        SaveInventory(defaultCars);
    }
    
    private List<Car> LoadInventory()
    {
        try
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<Car>>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading inventory: " + ex.Message);
            return new List<Car>();
        }
    }
    
    private void SaveInventory(List<Car> cars)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(cars, options);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving inventory: " + ex.Message);
        }
    }
    
    private void DisplayMainMenu()
    {
        bool exit = false;
        
        while (!exit)
        {
            Console.WriteLine("\nCar Rental Inventory Management");
            Console.WriteLine("1. View All Cars");
            Console.WriteLine("2. Add New Car");
            Console.WriteLine("3. Update Car Availability");
            Console.WriteLine("4. Remove Car");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    ViewAllCars();
                    break;
                case "2":
                    AddNewCar();
                    break;
                case "3":
                    UpdateCarAvailability();
                    break;
                case "4":
                    RemoveCar();
                    break;
                case "5":
                    exit = true;
                    Console.WriteLine("Exiting Car Rental Inventory Manager...");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    
    private void ViewAllCars()
    {
        var cars = LoadInventory();
        
        if (cars.Count == 0)
        {
            Console.WriteLine("No cars in inventory.");
            return;
        }
        
        Console.WriteLine("\nCurrent Inventory:");
        Console.WriteLine("ID\tMake\tModel\tYear\tDaily Rate\tAvailable");
        
        foreach (var car in cars)
        {
            Console.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4:C}\t{5}", 
                car.Id, car.Make, car.Model, car.Year, car.DailyRate, car.IsAvailable ? "Yes" : "No"));
        }
    }
    
    private void AddNewCar()
    {
        Console.WriteLine("\nAdd New Car");
        
        try
        {
            Console.Write("Enter Make: ");
            string make = Console.ReadLine();
            
            Console.Write("Enter Model: ");
            string model = Console.ReadLine();
            
            Console.Write("Enter Year: ");
            int year = int.Parse(Console.ReadLine());
            
            Console.Write("Enter Daily Rate: ");
            decimal dailyRate = decimal.Parse(Console.ReadLine());
            
            var cars = LoadInventory();
            int newId = cars.Count > 0 ? cars.Max(c => c.Id) + 1 : 1;
            
            cars.Add(new Car 
            { 
                Id = newId, 
                Make = make, 
                Model = model, 
                Year = year, 
                DailyRate = dailyRate, 
                IsAvailable = true 
            });
            
            SaveInventory(cars);
            Console.WriteLine("Car added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding car: " + ex.Message);
        }
    }
    
    private void UpdateCarAvailability()
    {
        ViewAllCars();
        
        try
        {
            Console.Write("\nEnter ID of car to update: ");
            int id = int.Parse(Console.ReadLine());
            
            var cars = LoadInventory();
            var car = cars.FirstOrDefault(c => c.Id == id);
            
            if (car == null)
            {
                Console.WriteLine("Car not found.");
                return;
            }
            
            Console.Write("Is the car available? (Y/N): ");
            string input = Console.ReadLine().ToUpper();
            
            car.IsAvailable = (input == "Y");
            SaveInventory(cars);
            
            Console.WriteLine("Availability updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating car: " + ex.Message);
        }
    }
    
    private void RemoveCar()
    {
        ViewAllCars();
        
        try
        {
            Console.Write("\nEnter ID of car to remove: ");
            int id = int.Parse(Console.ReadLine());
            
            var cars = LoadInventory();
            var car = cars.FirstOrDefault(c => c.Id == id);
            
            if (car == null)
            {
                Console.WriteLine("Car not found.");
                return;
            }
            
            cars.Remove(car);
            SaveInventory(cars);
            
            Console.WriteLine("Car removed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error removing car: " + ex.Message);
        }
    }
}

public class Car
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal DailyRate { get; set; }
    public bool IsAvailable { get; set; }
}