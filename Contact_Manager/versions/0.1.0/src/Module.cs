using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ContactManager : IGeneratedModule
{
    public string Name { get; set; } = "Contact Manager";
    
    private List<Contact> contacts;
    private string dataFilePath;
    
    public ContactManager()
    {
        contacts = new List<Contact>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Contact Manager module is running.");
        
        dataFilePath = Path.Combine(dataFolder, "contacts.json");
        
        try
        {
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                contacts = JsonSerializer.Deserialize<List<Contact>>(json);
            }
            
            bool exit = false;
            while (!exit)
            {
                DisplayMenu();
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddContact();
                        break;
                    case "2":
                        ListContacts();
                        break;
                    case "3":
                        SearchContact();
                        break;
                    case "4":
                        DeleteContact();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            SaveContacts();
            Console.WriteLine("Contacts saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nContact Manager Menu:");
        Console.WriteLine("1. Add Contact");
        Console.WriteLine("2. List Contacts");
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
        
        contacts.Add(new Contact { Name = name, Phone = phone, Email = email });
        Console.WriteLine("Contact added successfully.");
    }
    
    private void ListContacts()
    {
        if (contacts.Count == 0)
        {
            Console.WriteLine("No contacts available.");
            return;
        }
        
        Console.WriteLine("\nContact List:");
        foreach (var contact in contacts)
        {
            Console.WriteLine("Name: " + contact.Name);
            Console.WriteLine("Phone: " + contact.Phone);
            Console.WriteLine("Email: " + contact.Email);
            Console.WriteLine("----------------------");
        }
    }
    
    private void SearchContact()
    {
        Console.Write("Enter contact name to search: ");
        string searchName = Console.ReadLine();
        
        var foundContacts = contacts.FindAll(c => c.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase));
        
        if (foundContacts.Count == 0)
        {
            Console.WriteLine("No contacts found with that name.");
            return;
        }
        
        Console.WriteLine("\nFound Contacts:");
        foreach (var contact in foundContacts)
        {
            Console.WriteLine("Name: " + contact.Name);
            Console.WriteLine("Phone: " + contact.Phone);
            Console.WriteLine("Email: " + contact.Email);
            Console.WriteLine("----------------------");
        }
    }
    
    private void DeleteContact()
    {
        Console.Write("Enter contact name to delete: ");
        string deleteName = Console.ReadLine();
        
        int removed = contacts.RemoveAll(c => c.Name.Equals(deleteName, StringComparison.OrdinalIgnoreCase));
        
        if (removed > 0)
        {
            Console.WriteLine(removed + " contact(s) removed successfully.");
        }
        else
        {
            Console.WriteLine("No contacts found with that name.");
        }
    }
    
    private void SaveContacts()
    {
        string json = JsonSerializer.Serialize(contacts);
        File.WriteAllText(dataFilePath, json);
    }
}

public class Contact
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}