using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GroceryListManager : IGeneratedModule
{
    public string Name { get; set; } = "Household Grocery List Manager";

    private string _groceryListPath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Household Grocery List Manager...");
        _groceryListPath = Path.Combine(dataFolder, "grocery_list.json");

        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            List<string> groceryList = LoadGroceryList();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nGrocery List Manager");
                Console.WriteLine("1. View Grocery List");
                Console.WriteLine("2. Add Item");
                Console.WriteLine("3. Remove Item");
                Console.WriteLine("4. Clear List");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");

                string input = Console.ReadLine();
                if (int.TryParse(input, out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            DisplayGroceryList(groceryList);
                            break;
                        case 2:
                            AddItem(groceryList);
                            break;
                        case 3:
                            RemoveItem(groceryList);
                            break;
                        case 4:
                            ClearList(groceryList);
                            break;
                        case 5:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }

            SaveGroceryList(groceryList);
            Console.WriteLine("Grocery list saved. Exiting...");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private List<string> LoadGroceryList()
    {
        if (File.Exists(_groceryListPath))
        {
            string json = File.ReadAllText(_groceryListPath);
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        return new List<string>();
    }

    private void SaveGroceryList(List<string> groceryList)
    {
        string json = JsonSerializer.Serialize(groceryList);
        File.WriteAllText(_groceryListPath, json);
    }

    private void DisplayGroceryList(List<string> groceryList)
    {
        if (groceryList.Count == 0)
        {
            Console.WriteLine("The grocery list is empty.");
            return;
        }

        Console.WriteLine("\nCurrent Grocery List:");
        for (int i = 0; i < groceryList.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + groceryList[i]);
        }
    }

    private void AddItem(List<string> groceryList)
    {
        Console.Write("Enter item to add: ");
        string item = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(item))
        {
            groceryList.Add(item);
            Console.WriteLine("Item added successfully.");
        }
        else
        {
            Console.WriteLine("Item cannot be empty.");
        }
    }

    private void RemoveItem(List<string> groceryList)
    {
        if (groceryList.Count == 0)
        {
            Console.WriteLine("The grocery list is empty.");
            return;
        }

        DisplayGroceryList(groceryList);
        Console.Write("Enter item number to remove: ");

        if (int.TryParse(Console.ReadLine(), out int itemNumber) && itemNumber > 0 && itemNumber <= groceryList.Count)
        {
            string removedItem = groceryList[itemNumber - 1];
            groceryList.RemoveAt(itemNumber - 1);
            Console.WriteLine("Removed: " + removedItem);
        }
        else
        {
            Console.WriteLine("Invalid item number.");
        }
    }

    private void ClearList(List<string> groceryList)
    {
        Console.Write("Are you sure you want to clear the entire list? (y/n): ");
        string confirmation = Console.ReadLine();

        if (confirmation.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            groceryList.Clear();
            Console.WriteLine("Grocery list cleared.");
        }
    }
}