using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BakeryOrderSystem : IGeneratedModule
{
    public string Name { get; set; } = "Bakery Order System";

    private string _ordersFilePath;
    private List<Order> _orders;

    public BakeryOrderSystem()
    {
        _orders = new List<Order>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Bakery Order System...");
        _ordersFilePath = Path.Combine(dataFolder, "orders.json");

        LoadOrders();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nBakery Order System Menu:");
            Console.WriteLine("1. Add New Order");
            Console.WriteLine("2. View All Orders");
            Console.WriteLine("3. Search Orders");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddNewOrder();
                    break;
                case "2":
                    ViewAllOrders();
                    break;
                case "3":
                    SearchOrders();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveOrders();
        Console.WriteLine("Bakery Order System has been saved. Exiting...");
        return true;
    }

    private void LoadOrders()
    {
        if (File.Exists(_ordersFilePath))
        {
            try
            {
                string json = File.ReadAllText(_ordersFilePath);
                _orders = JsonSerializer.Deserialize<List<Order>>(json);
                Console.WriteLine("Previous orders loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading orders: " + ex.Message);
                _orders = new List<Order>();
            }
        }
        else
        {
            Console.WriteLine("No previous orders found. Starting with empty order list.");
        }
    }

    private void SaveOrders()
    {
        try
        {
            string json = JsonSerializer.Serialize(_orders, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_ordersFilePath, json);
            Console.WriteLine("Orders saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving orders: " + ex.Message);
        }
    }

    private void AddNewOrder()
    {
        Console.WriteLine("\nAdd New Order");
        Console.Write("Customer Name: ");
        string customerName = Console.ReadLine();

        Console.WriteLine("\nAvailable Products:");
        Console.WriteLine("1. Cake");
        Console.WriteLine("2. Pastry");
        Console.Write("Select product type (1-2): ");
        string productType = Console.ReadLine();

        Product product;
        if (productType == "1")
        {
            product = new Cake();
        }
        else if (productType == "2")
        {
            product = new Pastry();
        }
        else
        {
            Console.WriteLine("Invalid product type. Order not created.");
            return;
        }

        product.Configure();

        Console.Write("Quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity. Order not created.");
            return;
        }

        Console.Write("Delivery Date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime deliveryDate))
        {
            Console.WriteLine("Invalid date format. Using today's date.");
            deliveryDate = DateTime.Today;
        }

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerName = customerName,
            Product = product,
            Quantity = quantity,
            OrderDate = DateTime.Now,
            DeliveryDate = deliveryDate,
            Status = "Pending"
        };

        _orders.Add(order);
        Console.WriteLine("Order added successfully!");
    }

    private void ViewAllOrders()
    {
        Console.WriteLine("\nAll Orders:");
        if (_orders.Count == 0)
        {
            Console.WriteLine("No orders found.");
            return;
        }

        foreach (var order in _orders)
        {
            DisplayOrderDetails(order);
        }
    }

    private void SearchOrders()
    {
        Console.WriteLine("\nSearch Orders");
        Console.Write("Enter customer name or product type to search: ");
        string searchTerm = Console.ReadLine().ToLower();

        var results = _orders.FindAll(o =>
            o.CustomerName.ToLower().Contains(searchTerm) ||
            o.Product.GetType().Name.ToLower().Contains(searchTerm));

        if (results.Count == 0)
        {
            Console.WriteLine("No matching orders found.");
            return;
        }

        Console.WriteLine("\nMatching Orders:");
        foreach (var order in results)
        {
            DisplayOrderDetails(order);
        }
    }

    private void DisplayOrderDetails(Order order)
    {
        Console.WriteLine("\nOrder ID: " + order.Id);
        Console.WriteLine("Customer: " + order.CustomerName);
        Console.WriteLine("Product: " + order.Product.GetType().Name);
        Console.WriteLine("Details: " + order.Product.GetDescription());
        Console.WriteLine("Quantity: " + order.Quantity);
        Console.WriteLine("Order Date: " + order.OrderDate.ToString("yyyy-MM-dd"));
        Console.WriteLine("Delivery Date: " + order.DeliveryDate.ToString("yyyy-MM-dd"));
        Console.WriteLine("Status: " + order.Status);
    }
}

public class Order
{
    public string Id { get; set; }
    public string CustomerName { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string Status { get; set; }
}

public abstract class Product
{
    public abstract void Configure();
    public abstract string GetDescription();
}

public class Cake : Product
{
    public string Flavor { get; set; } = "Vanilla";
    public string Frosting { get; set; } = "Buttercream";
    public List<string> Toppings { get; set; } = new List<string>();
    public string Size { get; set; } = "Medium";

    public override void Configure()
    {
        Console.WriteLine("\nConfiguring Cake:");
        Console.Write("Flavor (Vanilla/Chocolate/Red Velvet): ");
        string flavorInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(flavorInput))
        {
            Flavor = flavorInput;
        }

        Console.Write("Frosting (Buttercream/Cream Cheese/Fondant): ");
        string frostingInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(frostingInput))
        {
            Frosting = frostingInput;
        }

        Console.WriteLine("Add toppings (enter one at a time, blank to finish):");
        while (true)
        {
            string topping = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(topping)) break;
            Toppings.Add(topping);
        }

        Console.Write("Size (Small/Medium/Large): ");
        string sizeInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(sizeInput))
        {
            Size = sizeInput;
        }
    }

    public override string GetDescription()
    {
        string description = Size + " " + Flavor + " cake with " + Frosting + " frosting";
        if (Toppings.Count > 0)
        {
            description += " and toppings: " + string.Join(", ", Toppings);
        }
        return description;
    }
}

public class Pastry : Product
{
    public string Type { get; set; } = "Croissant";
    public string Filling { get; set; } = "None";
    public bool Glazed { get; set; } = false;

    public override void Configure()
    {
        Console.WriteLine("\nConfiguring Pastry:");
        Console.Write("Type (Croissant/Danish/Muffin/Scone): ");
        string typeInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(typeInput))
        {
            Type = typeInput;
        }

        Console.Write("Filling (None/Chocolate/Fruit/Cream): ");
        string fillingInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(fillingInput))
        {
            Filling = fillingInput;
        }

        Console.Write("Glazed (Y/N): ");
        string glazedInput = Console.ReadLine();
        Glazed = glazedInput?.ToUpper() == "Y";
    }

    public override string GetDescription()
    {
        string description = Type + " pastry";
        if (Filling != "None")
        {
            description += " with " + Filling + " filling";
        }
        if (Glazed)
        {
            description += " and glazed";
        }
        return description;
    }
}