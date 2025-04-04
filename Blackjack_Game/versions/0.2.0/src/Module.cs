using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BlackjackModule
{
    public string Name { get; set; } = "Blackjack Game";
    private List<Card> deck;
    private List<Card> playerHand;
    private List<Card> dealerHand;
    private Random random;
    private string dataFolder;

    public BlackjackModule()
    {
        random = new Random();
        deck = new List<Card>();
        playerHand = new List<Card>();
        dealerHand = new List<Card>();
    }

    public bool Main(string dataFolder)
    {
        this.dataFolder = dataFolder;
        Console.WriteLine("Starting Blackjack Game...");
        InitializeDeck();
        ShuffleDeck();

        DealInitialCards();
        DisplayHands(false);

        PlayerTurn();
        if (CalculateHandValue(playerHand) > 21)
        {
            Console.WriteLine("Player busts! Dealer wins.");
            SaveGameResult("Dealer wins - Player busts.");
            return true;
        }

        DealerTurn();
        DisplayHands(true);

        DetermineWinner();
        return true;
    }

    private void InitializeDeck()
    {
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                deck.Add(new Card(suit, rank));
            }
        }
    }

    private void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            Card temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    private void DealInitialCards()
    {
        playerHand.Add(DrawCard());
        dealerHand.Add(DrawCard());
        playerHand.Add(DrawCard());
        dealerHand.Add(DrawCard());
    }

    private Card DrawCard()
    {
        if (deck.Count == 0)
        {
            throw new InvalidOperationException("Deck is empty");
        }

        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    private void PlayerTurn()
    {
        while (true)
        {
            Console.WriteLine("\nDo you want to hit (h) or stand (s)?");
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

    private int CalculateHandValue(List<Card> hand)
    {
        int value = 0;
        int aces = 0;

        foreach (var card in hand)
        {
            if (card.Rank == "Ace")
            {
                aces++;
                value += 11;
            }
            else if (card.Rank == "Jack" || card.Rank == "Queen" || card.Rank == "King")
            {
                value += 10;
            }
            else
            {
                value += int.Parse(card.Rank);
            }
        }

        while (value > 21 && aces > 0)
        {
            value -= 10;
            aces--;
        }

        return value;
    }

    private void DisplayHands(bool showDealerCard)
    {
        Console.WriteLine("\nDealer's Hand:");
        if (showDealerCard)
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

        Console.WriteLine("\nPlayer's Hand:");
        foreach (var card in playerHand)
        {
            Console.WriteLine(card);
        }
        Console.WriteLine("Total: " + CalculateHandValue(playerHand));
    }

    private void DetermineWinner()
    {
        int playerValue = CalculateHandValue(playerHand);
        int dealerValue = CalculateHandValue(dealerHand);

        if (dealerValue > 21)
        {
            Console.WriteLine("Dealer busts! Player wins.");
            SaveGameResult("Player wins - Dealer busts.");
        }
        else if (playerValue > dealerValue)
        {
            Console.WriteLine("Player wins!");
            SaveGameResult("Player wins - Higher score.");
        }
        else if (playerValue < dealerValue)
        {
            Console.WriteLine("Dealer wins!");
            SaveGameResult("Dealer wins - Higher score.");
        }
        else
        {
            Console.WriteLine("It's a tie!");
            SaveGameResult("Game tied.");
        }
    }

    private void SaveGameResult(string result)
    {
        try
        {
            string filePath = Path.Combine(dataFolder, "blackjack_results.json");
            List<string> results = new List<string>();

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                results = JsonSerializer.Deserialize<List<string>>(json);
            }

            results.Add($"{DateTime.Now}: {result}");
            string newJson = JsonSerializer.Serialize(results);
            File.WriteAllText(filePath, newJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving game result: " + ex.Message);
        }
    }
}

public class Card
{
    public string Suit { get; }
    public string Rank { get; }

    public Card(string suit, string rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public override string ToString()
    {
        return Rank + " of " + Suit;
    }
}