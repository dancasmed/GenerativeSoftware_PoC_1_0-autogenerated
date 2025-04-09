using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ContactManager : IGeneratedModule
{
    public string Name { get; set; } = "Contact Manager";
    private List<Contact> contacts = new List<Contact>();
    private string dataFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Contact Manager Module is running...");
        dataFilePath = Path.Combine(dataFolder, "contacts.json");
        LoadContacts();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n1. Add Contact");
            Console.WriteLine("2. View Contacts");
            Console.WriteLine("3. Delete Contact");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

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
                    DeleteContact();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveContacts();
        Console.WriteLine("Contact Manager Module finished.");
        return true;
    }

    private void AddContact()
    {
        Console.Write("Enter name: ");
        string name = Console.ReadLine();

        Console.Write("Enter phone number: ");
        string phoneNumber = Console.ReadLine();

        Console.Write("Enter email: ");
        string email = Console.ReadLine();

        contacts.Add(new Contact { Name = name, PhoneNumber = phoneNumber, Email = email });
        Console.WriteLine("Contact added successfully.");
    }

    private void ViewContacts()
    {
        if (contacts.Count == 0)
        {
            Console.WriteLine("No contacts available.");
            return;
        }

        Console.WriteLine("\nContacts:");
        for (int i = 0; i < contacts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. Name: {contacts[i].Name}, Phone: {contacts[i].PhoneNumber}, Email: {contacts[i].Email}");
        }
    }

    private void DeleteContact()
    {
        ViewContacts();
        if (contacts.Count == 0) return;

        Console.Write("Enter the number of the contact to delete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= contacts.Count)
        {
            contacts.RemoveAt(index - 1);
            Console.WriteLine("Contact deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }
    }

    private void LoadContacts()
    {
        if (File.Exists(dataFilePath))
        {
            string json = File.ReadAllText(dataFilePath);
            contacts = JsonSerializer.Deserialize<List<Contact>>(json);
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
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}