using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PackingListGenerator : IGeneratedModule
{
    public string Name { get; set; } = "International Travel Packing List Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Generating international travel packing list...");
        
        try
        {
            var packingList = new PackingList
            {
                Essentials = new List<string>
                {
                    "Passport",
                    "Visa documents",
                    "Travel insurance",
                    "Credit cards/cash",
                    "Emergency contacts"
                },
                Clothing = new List<string>
                {
                    "Underwear (5-7 pairs)",
                    "Socks (5-7 pairs)",
                    "T-shirts (5-7)",
                    "Pants/shorts (3-5)",
                    "Sweater/jacket",
                    "Sleepwear",
                    "Swimwear",
                    "Comfortable walking shoes",
                    "Dress shoes (if needed)"
                },
                Toiletries = new List<string>
                {
                    "Toothbrush & toothpaste",
                    "Deodorant",
                    "Shampoo & conditioner",
                    "Razor & shaving cream",
                    "Hairbrush/comb",
                    "Nail clippers",
                    "Prescription medications",
                    "First aid kit"
                },
                Electronics = new List<string>
                {
                    "Phone & charger",
                    "Laptop/tablet & charger",
                    "Universal power adapter",
                    "Headphones",
                    "Camera"
                },
                Miscellaneous = new List<string>
                {
                    "Travel pillow",
                    "Earplugs",
                    "Eye mask",
                    "Reusable water bottle",
                    "Snacks",
                    "Books/magazines",
                    "Travel guide"
                }
            };

            string json = JsonSerializer.Serialize(packingList, new JsonSerializerOptions { WriteIndented = true });
            string filePath = Path.Combine(dataFolder, "packing_list.json");
            
            File.WriteAllText(filePath, json);
            Console.WriteLine("Packing list generated successfully at: " + filePath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating packing list: " + ex.Message);
            return false;
        }
    }

    private class PackingList
    {
        public List<string> Essentials { get; set; }
        public List<string> Clothing { get; set; }
        public List<string> Toiletries { get; set; }
        public List<string> Electronics { get; set; }
        public List<string> Miscellaneous { get; set; }
    }
}