using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GymMembershipManager : IGeneratedModule
{
    public string Name { get; set; } = "Gym Membership Manager";
    
    private string _membersFilePath;
    
    public bool Main(string dataFolder)
    {
        _membersFilePath = Path.Combine(dataFolder, "members.json");
        
        Console.WriteLine("Gym Membership Manager is running.");
        Console.WriteLine("Initializing database in: " + dataFolder);
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<Member> members = LoadMembers();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    AddMember(members);
                    break;
                case "2":
                    ViewMembers(members);
                    break;
                case "3":
                    SearchMember(members);
                    break;
                case "4":
                    DeleteMember(members);
                    break;
                case "5":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
            
            SaveMembers(members);
        }
        
        Console.WriteLine("Gym Membership Manager is shutting down.");
        return true;
    }
    
    private List<Member> LoadMembers()
    {
        if (File.Exists(_membersFilePath))
        {
            string json = File.ReadAllText(_membersFilePath);
            return JsonSerializer.Deserialize<List<Member>>(json) ?? new List<Member>();
        }
        return new List<Member>();
    }
    
    private void SaveMembers(List<Member> members)
    {
        string json = JsonSerializer.Serialize(members);
        File.WriteAllText(_membersFilePath, json);
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nGym Membership Manager");
        Console.WriteLine("1. Add Member");
        Console.WriteLine("2. View All Members");
        Console.WriteLine("3. Search Member");
        Console.WriteLine("4. Delete Member");
        Console.WriteLine("5. Exit");
        Console.Write("Enter your choice: ");
    }
    
    private void AddMember(List<Member> members)
    {
        Console.Write("Enter member name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter member ID: ");
        string memberId = Console.ReadLine();
        
        Console.Write("Enter membership type: ");
        string membershipType = Console.ReadLine();
        
        Console.Write("Enter start date (YYYY-MM-DD): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
        {
            members.Add(new Member
            {
                Name = name,
                MemberId = memberId,
                MembershipType = membershipType,
                StartDate = startDate,
                IsActive = true
            });
            
            Console.WriteLine("Member added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid date format. Member not added.");
        }
    }
    
    private void ViewMembers(List<Member> members)
    {
        if (members.Count == 0)
        {
            Console.WriteLine("No members found.");
            return;
        }
        
        Console.WriteLine("\nList of Members:");
        foreach (var member in members)
        {
            Console.WriteLine($"ID: {member.MemberId}, Name: {member.Name}, Type: {member.MembershipType}, Start Date: {member.StartDate.ToShortDateString()}, Active: {member.IsActive}");
        }
    }
    
    private void SearchMember(List<Member> members)
    {
        Console.Write("Enter member name or ID to search: ");
        string searchTerm = Console.ReadLine();
        
        var foundMembers = members.FindAll(m => 
            m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
            m.MemberId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        
        if (foundMembers.Count == 0)
        {
            Console.WriteLine("No members found matching your search.");
        }
        else
        {
            Console.WriteLine("\nFound Members:");
            foreach (var member in foundMembers)
            {
                Console.WriteLine($"ID: {member.MemberId}, Name: {member.Name}, Type: {member.MembershipType}, Start Date: {member.StartDate.ToShortDateString()}, Active: {member.IsActive}");
            }
        }
    }
    
    private void DeleteMember(List<Member> members)
    {
        Console.Write("Enter member ID to delete: ");
        string memberId = Console.ReadLine();
        
        int index = members.FindIndex(m => m.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase));
        
        if (index >= 0)
        {
            members.RemoveAt(index);
            Console.WriteLine("Member deleted successfully.");
        }
        else
        {
            Console.WriteLine("Member not found.");
        }
    }
}

public class Member
{
    public string Name { get; set; }
    public string MemberId { get; set; }
    public string MembershipType { get; set; }
    public DateTime StartDate { get; set; }
    public bool IsActive { get; set; }
}