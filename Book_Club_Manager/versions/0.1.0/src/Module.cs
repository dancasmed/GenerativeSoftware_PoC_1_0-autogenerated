using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BookClubManager : IGeneratedModule
{
    public string Name { get; set; } = "Book Club Manager";

    private string _meetingsFilePath;
    private string _suggestionsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Book Club Manager module is running.");

        _meetingsFilePath = Path.Combine(dataFolder, "meetings.json");
        _suggestionsFilePath = Path.Combine(dataFolder, "suggestions.json");

        EnsureDataFilesExist();

        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ScheduleMeeting();
                    break;
                case "2":
                    ListMeetings();
                    break;
                case "3":
                    SuggestBook();
                    break;
                case "4":
                    ListSuggestions();
                    break;
                case "5":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        return true;
    }

    private void EnsureDataFilesExist()
    {
        if (!File.Exists(_meetingsFilePath))
        {
            File.WriteAllText(_meetingsFilePath, "[]");
        }

        if (!File.Exists(_suggestionsFilePath))
        {
            File.WriteAllText(_suggestionsFilePath, "[]");
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nBook Club Manager");
        Console.WriteLine("1. Schedule a meeting");
        Console.WriteLine("2. List scheduled meetings");
        Console.WriteLine("3. Suggest a book");
        Console.WriteLine("4. List book suggestions");
        Console.WriteLine("5. Exit module");
        Console.Write("Enter your choice: ");
    }

    private void ScheduleMeeting()
    {
        Console.Write("Enter meeting date (yyyy-MM-dd): ");
        var dateInput = Console.ReadLine();

        Console.Write("Enter meeting time (HH:mm): ");
        var timeInput = Console.ReadLine();

        Console.Write("Enter discussion topic: ");
        var topic = Console.ReadLine();

        if (DateTime.TryParse(dateInput + " " + timeInput, out var meetingDateTime))
        {
            var meetings = GetMeetings();
            meetings.Add(new Meeting
            {
                DateTime = meetingDateTime,
                Topic = topic
            });

            SaveMeetings(meetings);
            Console.WriteLine("Meeting scheduled successfully.");
        }
        else
        {
            Console.WriteLine("Invalid date or time format.");
        }
    }

    private void ListMeetings()
    {
        var meetings = GetMeetings();

        if (meetings.Count == 0)
        {
            Console.WriteLine("No meetings scheduled.");
            return;
        }

        Console.WriteLine("\nScheduled Meetings:");
        foreach (var meeting in meetings)
        {
            Console.WriteLine($"{meeting.DateTime:yyyy-MM-dd HH:mm} - {meeting.Topic}");
        }
    }

    private void SuggestBook()
    {
        Console.Write("Enter book title: ");
        var title = Console.ReadLine();

        Console.Write("Enter author: ");
        var author = Console.ReadLine();

        Console.Write("Enter your name: ");
        var suggestedBy = Console.ReadLine();

        var suggestions = GetSuggestions();
        suggestions.Add(new BookSuggestion
        {
            Title = title,
            Author = author,
            SuggestedBy = suggestedBy,
            SuggestedOn = DateTime.Now
        });

        SaveSuggestions(suggestions);
        Console.WriteLine("Book suggestion added successfully.");
    }

    private void ListSuggestions()
    {
        var suggestions = GetSuggestions();

        if (suggestions.Count == 0)
        {
            Console.WriteLine("No book suggestions available.");
            return;
        }

        Console.WriteLine("\nBook Suggestions:");
        foreach (var suggestion in suggestions)
        {
            Console.WriteLine($"{suggestion.Title} by {suggestion.Author} (suggested by {suggestion.SuggestedBy} on {suggestion.SuggestedOn:yyyy-MM-dd})");
        }
    }

    private List<Meeting> GetMeetings()
    {
        var json = File.ReadAllText(_meetingsFilePath);
        return JsonSerializer.Deserialize<List<Meeting>>(json) ?? new List<Meeting>();
    }

    private void SaveMeetings(List<Meeting> meetings)
    {
        var json = JsonSerializer.Serialize(meetings);
        File.WriteAllText(_meetingsFilePath, json);
    }

    private List<BookSuggestion> GetSuggestions()
    {
        var json = File.ReadAllText(_suggestionsFilePath);
        return JsonSerializer.Deserialize<List<BookSuggestion>>(json) ?? new List<BookSuggestion>();
    }

    private void SaveSuggestions(List<BookSuggestion> suggestions)
    {
        var json = JsonSerializer.Serialize(suggestions);
        File.WriteAllText(_suggestionsFilePath, json);
    }

    private class Meeting
    {
        public DateTime DateTime { get; set; }
        public string Topic { get; set; }
    }

    private class BookSuggestion
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string SuggestedBy { get; set; }
        public DateTime SuggestedOn { get; set; }
    }
}