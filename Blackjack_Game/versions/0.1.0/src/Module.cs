using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BlackjackModule : IGeneratedModule
{
    public string Name { get; set; } = "Blackjack Game";

    private List<string> deck;
    private List<string> playerHand;
    private List<string> dealerHand;
    private Random random;
    private string dataFolder;

    public BlackjackModule()
    {
        random = new Random();
    }

    public bool Main(string dataFolder)
    {
        this.dataFolder = dataFolder;
        InitializeDeck();
        playerHand = new List<string>();
        dealerHand = new List<string>();

        Console.WriteLine("Starting Blackjack Game...");
        Console.WriteLine("Dealing initial cards...");

        DealInitialCards();
        DisplayHands(false);

        PlayerTurn();
        if (CalculateHandValue(playerHand) > 21)
        {
            Console.WriteLine("Player busts! Dealer wins.");
            SaveGameResult(false);
            return true;
        }

        DealerTurn();
        DisplayHands(true);

        int playerValue = CalculateHandValue(playerHand);
        int dealerValue = CalculateHandValue(dealerHand);

        if (dealerValue > 21)
        {
            Console.WriteLine("Dealer busts! Player wins.");
            SaveGameResult(true);
        }
        else if (playerValue > dealerValue)
        {
            Console.WriteLine("Player wins!");
            SaveGameResult(true);
        }
        else if (playerValue < dealerValue)
        {
            Console.WriteLine("Dealer wins!");
            SaveGameResult(false);
        }
        else
        {
            Console.WriteLine("It's a tie!");
        }

        return true;
    }

    private void InitializeDeck()
    {
        deck = new List<string>();
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                deck.Add(rank + " of " + suit);
            }
        }
    }

    private void DealInitialCards()
    {
        playerHand.Add(DrawCard());
        dealerHand.Add(DrawCard());
        playerHand.Add(DrawCard());
        dealerHand.Add(DrawCard());
    }

    private string DrawCard()
    {
        int index = random.Next(deck.Count);
        string card = deck[index];
        deck.RemoveAt(index);
        return card;
    }

    private int CalculateHandValue(List<string> hand)
    {
        int value = 0;
        int aces = 0;

        foreach (var card in hand)
        {
            string rank = card.Split(' ')[0];
            if (rank == "Ace")
            {
                value += 11;
                aces++;
            }
            else if (rank == "Jack" || rank == "Queen" || rank == "King")
            {
                value += 10;
            }
            else
            {
                value += int.Parse(rank);
            }
        }

        while (value > 21 && aces > 0)
        {
            value -= 10;
            aces--;
        }

        return value;
    }

    private void PlayerTurn()
    {
        while (true)
        {
            Console.WriteLine("\nDo you want to Hit (h) or Stand (s)?");
            string input = Console.ReadLine().ToLower();

            if (input == "h")
            {
                playerHand.Add(DrawCard());
                DisplayHands(false);
                if (CalculateHandValue(playerHand) > 21)
                {
                    return;
                }
            }
            else if (input == "s")
            {
                return;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 'h' or 's'.");
            }
        }
    }

    private void DealerTurn()
    {
        while (CalculateHandValue(dealerHand) < 17)
        {
            dealerHand.Add(DrawCard());
        }
    }

    private void DisplayHands(bool showDealerHand)
    {
        Console.WriteLine("\nPlayer's Hand:");
        foreach (var card in playerHand)
        {
            Console.WriteLine(card);
        }
        Console.WriteLine("Total: " + CalculateHandValue(playerHand));

        Console.WriteLine("\nDealer's Hand:");
        if (showDealerHand)
        {
            foreach (var card in dealerHand)
            {
                Console.WriteLine(card);
            }
            Console.WriteLine("Total: " + CalculateHandValue(dealerHand));
        }
        else
        {
            Console.WriteLine(dealerHand[0]);
            Console.WriteLine("[Hidden Card]");
        }
    }

    private void SaveGameResult(bool playerWon)
    {
        try
        {
            string filePath = Path.Combine(dataFolder, "blackjack_results.json");
            var results = new List<GameResult>();

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                results = JsonSerializer.Deserialize<List<GameResult>>(json);
            }

            results.Add(new GameResult
            {
                PlayerWon = playerWon,
                PlayerScore = CalculateHandValue(playerHand),
                DealerScore = CalculateHandValue(dealerHand),
                Date = DateTime.Now
            });

            string newJson = JsonSerializer.Serialize(results);
            File.WriteAllText(filePath, newJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving game result: " + ex.Message);
        }
    }

    private class GameResult
    {
        public bool PlayerWon { get; set; }
        public int PlayerScore { get; set; }
        public int DealerScore { get; set; }
        public DateTime Date { get; set; }
    }
}