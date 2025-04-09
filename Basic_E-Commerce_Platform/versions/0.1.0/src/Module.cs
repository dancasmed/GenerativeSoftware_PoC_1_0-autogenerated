using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ECommerceModule : IGeneratedModule
{
    public string Name { get; set; } = "Basic E-Commerce Platform";

    private List<Product> products = new List<Product>();
    private List<CartItem> cart = new List<CartItem>();
    private string productsFilePath;
    private string cartFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing E-Commerce Module...");
        
        productsFilePath = Path.Combine(dataFolder, "products.json");
        cartFilePath = Path.Combine(dataFolder, "cart.json");

        LoadProducts();
        LoadCart();

        bool running = true;
        while (running)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    ListProducts();
                    break;
                case "2":
                    AddToCart();
                    break;
                case "3":
                    ViewCart();
                    break;
                case "4":
                    Checkout();
                    running = false;
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveProducts();
        SaveCart();
        Console.WriteLine("E-Commerce Module completed successfully.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nE-Commerce Platform");
        Console.WriteLine("1. List Products");
        Console.WriteLine("2. Add Product to Cart");
        Console.WriteLine("3. View Cart");
        Console.WriteLine("4. Checkout");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void LoadProducts()
    {
        if (File.Exists(productsFilePath))
        {
            string json = File.ReadAllText(productsFilePath);
            products = JsonSerializer.Deserialize<List<Product>>(json);
        }
        else
        {
            // Initialize with some sample products
            products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 10 },
                new Product { Id = 2, Name = "Smartphone", Price = 699.99m, Stock = 15 },
                new Product { Id = 3, Name = "Headphones", Price = 149.99m, Stock = 20 },
                new Product { Id = 4, Name = "Keyboard", Price = 59.99m, Stock = 30 },
                new Product { Id = 5, Name = "Mouse", Price = 29.99m, Stock = 40 }
            };
        }
    }

    private void SaveProducts()
    {
        string json = JsonSerializer.Serialize(products);
        File.WriteAllText(productsFilePath, json);
    }

    private void LoadCart()
    {
        if (File.Exists(cartFilePath))
        {
            string json = File.ReadAllText(cartFilePath);
            cart = JsonSerializer.Deserialize<List<CartItem>>(json);
        }
    }

    private void SaveCart()
    {
        string json = JsonSerializer.Serialize(cart);
        File.WriteAllText(cartFilePath, json);
    }

    private void ListProducts()
    {
        Console.WriteLine("\nAvailable Products:");
        Console.WriteLine("ID\tName\t\tPrice\tStock");
        foreach (var product in products)
        {
            Console.WriteLine(product.Id + "\t" + product.Name + "\t\t" + product.Price + "\t" + product.Stock);
        }
    }

    private void AddToCart()
    {
        Console.Write("Enter Product ID: ");
        if (int.TryParse(Console.ReadLine(), out int productId))
        {
            Console.Write("Enter Quantity: ");
            if (int.TryParse(Console.ReadLine(), out int quantity))
            {
                var product = products.Find(p => p.Id == productId);
                if (product != null && product.Stock >= quantity)
                {
                    var existingItem = cart.Find(item => item.ProductId == productId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                    }
                    else
                    {
                        cart.Add(new CartItem { ProductId = productId, Name = product.Name, Price = product.Price, Quantity = quantity });
                    }
                    product.Stock -= quantity;
                    Console.WriteLine("Product added to cart successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid product ID or insufficient stock.");
                }
            }
            else
            {
                Console.WriteLine("Invalid quantity.");
            }
        }
        else
        {
            Console.WriteLine("Invalid product ID.");
        }
    }

    private void ViewCart()
    {
        if (cart.Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
            return;
        }

        Console.WriteLine("\nYour Cart:");
        Console.WriteLine("Name\t\tPrice\tQuantity\tTotal");
        decimal cartTotal = 0;
        foreach (var item in cart)
        {
            decimal itemTotal = item.Price * item.Quantity;
            cartTotal += itemTotal;
            Console.WriteLine(item.Name + "\t\t" + item.Price + "\t" + item.Quantity + "\t\t" + itemTotal);
        }
        Console.WriteLine("\nCart Total: " + cartTotal);
    }

    private void Checkout()
    {
        if (cart.Count == 0)
        {
            Console.WriteLine("Your cart is empty. Nothing to checkout.");
            return;
        }

        ViewCart();
        Console.WriteLine("\nProcessing your order...");
        cart.Clear();
        Console.WriteLine("Order completed successfully. Thank you for your purchase!");
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class CartItem
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}