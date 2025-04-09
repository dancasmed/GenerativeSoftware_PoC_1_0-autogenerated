using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CafeManager : IGeneratedModule
{
    public string Name { get; set; } = "Cafe Manager";

    private string menuFilePath;
    private string reservationsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Cafe Manager module...");
        
        menuFilePath = Path.Combine(dataFolder, "menu.json");
        reservationsFilePath = Path.Combine(dataFolder, "reservations.json");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        InitializeFiles();

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nCafe Management System");
            Console.WriteLine("1. View Menu");
            Console.WriteLine("2. Add Item to Menu");
            Console.WriteLine("3. View Reservations");
            Console.WriteLine("4. Make Reservation");
            Console.WriteLine("5. Exit Module");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    ViewMenu();
                    break;
                case "2":
                    AddMenuItem();
                    break;
                case "3":
                    ViewReservations();
                    break;
                case "4":
                    MakeReservation();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Cafe Manager module finished.");
        return true;
    }

    private void InitializeFiles()
    {
        if (!File.Exists(menuFilePath))
        {
            File.WriteAllText(menuFilePath, "[]");
        }

        if (!File.Exists(reservationsFilePath))
        {
            File.WriteAllText(reservationsFilePath, "[]");
        }
    }

    private void ViewMenu()
    {
        try
        {
            string json = File.ReadAllText(menuFilePath);
            var menuItems = JsonSerializer.Deserialize<List<MenuItem>>(json);

            Console.WriteLine("\n--- Menu ---");
            foreach (var item in menuItems)
            {
                Console.WriteLine(item.ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error viewing menu: " + ex.Message);
        }
    }

    private void AddMenuItem()
    {
        try
        {
            Console.Write("Enter item name: ");
            string name = Console.ReadLine();

            Console.Write("Enter item description: ");
            string description = Console.ReadLine();

            Console.Write("Enter item price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price format.");
                return;
            }

            string json = File.ReadAllText(menuFilePath);
            var menuItems = JsonSerializer.Deserialize<List<MenuItem>>(json);

            menuItems.Add(new MenuItem
            {
                Id = menuItems.Count + 1,
                Name = name,
                Description = description,
                Price = price
            });

            File.WriteAllText(menuFilePath, JsonSerializer.Serialize(menuItems));
            Console.WriteLine("Menu item added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding menu item: " + ex.Message);
        }
    }

    private void ViewReservations()
    {
        try
        {
            string json = File.ReadAllText(reservationsFilePath);
            var reservations = JsonSerializer.Deserialize<List<Reservation>>(json);

            Console.WriteLine("\n--- Reservations ---");
            foreach (var reservation in reservations)
            {
                Console.WriteLine(reservation.ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error viewing reservations: " + ex.Message);
        }
    }

    private void MakeReservation()
    {
        try
        {
            Console.Write("Enter customer name: ");
            string customerName = Console.ReadLine();

            Console.Write("Enter reservation date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.Write("Enter number of guests: ");
            if (!int.TryParse(Console.ReadLine(), out int guests) || guests <= 0)
            {
                Console.WriteLine("Invalid number of guests.");
                return;
            }

            string json = File.ReadAllText(reservationsFilePath);
            var reservations = JsonSerializer.Deserialize<List<Reservation>>(json);

            reservations.Add(new Reservation
            {
                Id = reservations.Count + 1,
                CustomerName = customerName,
                Date = date,
                NumberOfGuests = guests
            });

            File.WriteAllText(reservationsFilePath, JsonSerializer.Serialize(reservations));
            Console.WriteLine("Reservation made successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error making reservation: " + ex.Message);
        }
    }
}

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public override string ToString()
    {
        return $"{Id}. {Name} - {Description} - ${Price}";
    }
}

public class Reservation
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateTime Date { get; set; }
    public int NumberOfGuests { get; set; }

    public override string ToString()
    {
        return $"{Id}. {CustomerName} - {Date.ToShortDateString()} - {NumberOfGuests} guests";
    }
}