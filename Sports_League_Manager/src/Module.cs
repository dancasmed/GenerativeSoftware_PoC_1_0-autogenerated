using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SportsLeagueManager : IGeneratedModule
{
    public string Name { get; set; } = "Sports League Manager";

    private string _teamsFilePath;
    private string _matchesFilePath;

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Initializing Sports League Manager...");
            
            _teamsFilePath = Path.Combine(dataFolder, "teams.json");
            _matchesFilePath = Path.Combine(dataFolder, "matches.json");

            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            if (!File.Exists(_teamsFilePath))
            {
                File.WriteAllText(_teamsFilePath, "[]");
            }

            if (!File.Exists(_matchesFilePath))
            {
                File.WriteAllText(_matchesFilePath, "[]");
            }

            bool exitRequested = false;
            while (!exitRequested)
            {
                Console.WriteLine("\nSports League Manager");
                Console.WriteLine("1. Add Team");
                Console.WriteLine("2. List Teams");
                Console.WriteLine("3. Schedule Match");
                Console.WriteLine("4. Record Match Result");
                Console.WriteLine("5. View Match Schedule");
                Console.WriteLine("6. View Standings");
                Console.WriteLine("7. Exit");
                Console.Write("Select an option: ");

                var input = Console.ReadLine();
                if (int.TryParse(input, out int option))
                {
                    switch (option)
                    {
                        case 1:
                            AddTeam();
                            break;
                        case 2:
                            ListTeams();
                            break;
                        case 3:
                            ScheduleMatch();
                            break;
                        case 4:
                            RecordMatchResult();
                            break;
                        case 5:
                            ViewMatchSchedule();
                            break;
                        case 6:
                            ViewStandings();
                            break;
                        case 7:
                            exitRequested = true;
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

            Console.WriteLine("Exiting Sports League Manager...");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void AddTeam()
    {
        Console.Write("Enter team name: ");
        var teamName = Console.ReadLine();

        var teams = LoadTeams();
        teams.Add(new Team { Name = teamName });
        SaveTeams(teams);

        Console.WriteLine("Team added successfully.");
    }

    private void ListTeams()
    {
        var teams = LoadTeams();
        Console.WriteLine("\nTeams:");
        foreach (var team in teams)
        {
            Console.WriteLine(team.Name);
        }
    }

    private void ScheduleMatch()
    {
        var teams = LoadTeams();
        if (teams.Count < 2)
        {
            Console.WriteLine("At least 2 teams are required to schedule a match.");
            return;
        }

        Console.WriteLine("Select home team:");
        for (int i = 0; i < teams.Count; i++)
        {
            Console.WriteLine(i + 1 + ". " + teams[i].Name);
        }
        var homeTeamIndex = GetValidTeamIndex(teams.Count);

        Console.WriteLine("Select away team:");
        for (int i = 0; i < teams.Count; i++)
        {
            Console.WriteLine(i + 1 + ". " + teams[i].Name);
        }
        var awayTeamIndex = GetValidTeamIndex(teams.Count);

        if (homeTeamIndex == awayTeamIndex)
        {
            Console.WriteLine("A team cannot play against itself.");
            return;
        }

        Console.Write("Enter match date (yyyy-MM-dd): ");
        var dateInput = Console.ReadLine();
        if (!DateTime.TryParse(dateInput, out DateTime matchDate))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }

        var matches = LoadMatches();
        matches.Add(new Match
        {
            HomeTeam = teams[homeTeamIndex].Name,
            AwayTeam = teams[awayTeamIndex].Name,
            Date = matchDate,
            IsCompleted = false
        });
        SaveMatches(matches);

        Console.WriteLine("Match scheduled successfully.");
    }

    private void RecordMatchResult()
    {
        var matches = LoadMatches();
        var incompleteMatches = matches.FindAll(m => !m.IsCompleted);

        if (incompleteMatches.Count == 0)
        {
            Console.WriteLine("No matches to record results for.");
            return;
        }

        Console.WriteLine("Select match to record result:");
        for (int i = 0; i < incompleteMatches.Count; i++)
        {
            Console.WriteLine(i + 1 + ". " + incompleteMatches[i].HomeTeam + " vs " + incompleteMatches[i].AwayTeam + " on " + incompleteMatches[i].Date.ToShortDateString());
        }

        if (!int.TryParse(Console.ReadLine(), out int matchIndex) || matchIndex < 1 || matchIndex > incompleteMatches.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        var selectedMatch = incompleteMatches[matchIndex - 1];

        Console.Write("Enter home team score: ");
        if (!int.TryParse(Console.ReadLine(), out int homeScore))
        {
            Console.WriteLine("Invalid score.");
            return;
        }

        Console.Write("Enter away team score: ");
        if (!int.TryParse(Console.ReadLine(), out int awayScore))
        {
            Console.WriteLine("Invalid score.");
            return;
        }

        selectedMatch.HomeScore = homeScore;
        selectedMatch.AwayScore = awayScore;
        selectedMatch.IsCompleted = true;

        SaveMatches(matches);
        Console.WriteLine("Match result recorded successfully.");
    }

    private void ViewMatchSchedule()
    {
        var matches = LoadMatches();
        Console.WriteLine("\nMatch Schedule:");
        foreach (var match in matches)
        {
            var status = match.IsCompleted ? "Completed" : "Scheduled";
            var result = match.IsCompleted ? " (" + match.HomeScore + "-" + match.AwayScore + ")" : "";
            Console.WriteLine(match.HomeTeam + " vs " + match.AwayTeam + " on " + match.Date.ToShortDateString() + " - " + status + result);
        }
    }

    private void ViewStandings()
    {
        var teams = LoadTeams();
        var matches = LoadMatches();

        foreach (var team in teams)
        {
            team.GamesPlayed = 0;
            team.Wins = 0;
            team.Draws = 0;
            team.Losses = 0;
            team.Points = 0;
        }

        foreach (var match in matches)
        {
            if (!match.IsCompleted) continue;

            var homeTeam = teams.Find(t => t.Name == match.HomeTeam);
            var awayTeam = teams.Find(t => t.Name == match.AwayTeam);

            if (homeTeam == null || awayTeam == null) continue;

            homeTeam.GamesPlayed++;
            awayTeam.GamesPlayed++;

            if (match.HomeScore > match.AwayScore)
            {
                homeTeam.Wins++;
                homeTeam.Points += 3;
                awayTeam.Losses++;
            }
            else if (match.HomeScore < match.AwayScore)
            {
                awayTeam.Wins++;
                awayTeam.Points += 3;
                homeTeam.Losses++;
            }
            else
            {
                homeTeam.Draws++;
                awayTeam.Draws++;
                homeTeam.Points++;
                awayTeam.Points++;
            }
        }

        teams.Sort((a, b) => b.Points.CompareTo(a.Points));

        Console.WriteLine("\nLeague Standings:");
        Console.WriteLine("Team\t\tGP\tW\tD\tL\tPts");
        foreach (var team in teams)
        {
            Console.WriteLine(team.Name + "\t\t" + team.GamesPlayed + "\t" + team.Wins + "\t" + team.Draws + "\t" + team.Losses + "\t" + team.Points);
        }
    }

    private List<Team> LoadTeams()
    {
        var json = File.ReadAllText(_teamsFilePath);
        return JsonSerializer.Deserialize<List<Team>>(json) ?? new List<Team>();
    }

    private void SaveTeams(List<Team> teams)
    {
        var json = JsonSerializer.Serialize(teams);
        File.WriteAllText(_teamsFilePath, json);
    }

    private List<Match> LoadMatches()
    {
        var json = File.ReadAllText(_matchesFilePath);
        return JsonSerializer.Deserialize<List<Match>>(json) ?? new List<Match>();
    }

    private void SaveMatches(List<Match> matches)
    {
        var json = JsonSerializer.Serialize(matches);
        File.WriteAllText(_matchesFilePath, json);
    }

    private int GetValidTeamIndex(int teamCount)
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= teamCount)
            {
                return index - 1;
            }
            Console.WriteLine("Invalid selection. Please try again.");
        }
    }
}

public class Team
{
    public string Name { get; set; }
    public int GamesPlayed { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int Points { get; set; }
}

public class Match
{
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public DateTime Date { get; set; }
    public bool IsCompleted { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}