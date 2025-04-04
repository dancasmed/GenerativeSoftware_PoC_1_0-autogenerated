using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ContactManager : IGeneratedModule
{
    public string Name { get; set; } = "Contact Manager";
    
    private List<Contact> _contacts = new List<Contact>();
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Contact Manager module...");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        _dataFilePath = Path.Combine(dataFolder, "contacts.json");
        
        LoadContacts();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddContact();
                    break;
                case "2":
                    ViewContacts();
                    break;
                case "3":
                    SearchContact();
                    break;
                case "4":
                    DeleteContact();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveContacts();
        Console.WriteLine("Contact Manager module finished.");
        
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nContact Manager Menu:");
        Console.WriteLine("1. Add Contact");
        Console.WriteLine("2. View Contacts");
        Console.WriteLine("3. Search Contact");
        Console.WriteLine("4. Delete Contact");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddContact()
    {
        Console.Write("Enter contact name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter phone number: ");
        string phone = Console.ReadLine();
        
        Console.Write("Enter email address: ");
        string email = Console.ReadLine();
        
        _contacts.Add(new Contact { Name = name, Phone = phone, Email = email });
        Console.WriteLine("Contact added successfully.");
    }
    
    private void ViewContacts()
    {
        if (_contacts.Count == 0)
        {
            Console.WriteLine("No contacts available.");
            return;
        }
        
        Console.WriteLine("\nContacts List:");
        for (int i = 0; i < _contacts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_contacts[i].Name} - {_contacts[i].Phone} - {_contacts[i].Email}");
        }
    }
    
    private void SearchContact()
    {
        Console.Write("Enter search term: ");
        string term = Console.ReadLine().ToLower();
        
        var results = _contacts.FindAll(c => 
            c.Name.ToLower().Contains(term) || 
            c.Phone.Contains(term) || 
            c.Email.ToLower().Contains(term));
        
        if (results.Count == 0)
        {
            Console.WriteLine("No matching contacts found.");
            return;
        }
        
        Console.WriteLine("\nSearch Results:");
        foreach (var contact in results)
        {
            Console.WriteLine($"{contact.Name} - {contact.Phone} - {contact.Email}");
        }
    }
    
    private void DeleteContact()
    {
        ViewContacts();
        
        if (_contacts.Count == 0)
            return;
        
        Console.Write("Enter contact number to delete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _contacts.Count)
        {
            _contacts.RemoveAt(index - 1);
            Console.WriteLine("Contact deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid contact number.");
        }
    }
    
    private void LoadContacts()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _contacts = JsonSerializer.Deserialize<List<Contact>>(json);
                Console.WriteLine("Contacts loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading contacts: " + ex.Message);
            }
        }
    }
    
    private void SaveContacts()
    {
        try
        {
            string json = JsonSerializer.Serialize(_contacts);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Contacts saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving contacts: " + ex.Message);
        }
    }
}

public class Contact
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}