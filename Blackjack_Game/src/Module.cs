using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BlackjackGame : IGeneratedModule
{
    public string Name { get; set; } = "Blackjack Game";
    private List<string> deck;
    private List<string> playerHand;
    private List<string> dealerHand;
    private Random random;
    private string dataFolder;

    public BlackjackGame()
    {
        random = new Random();
        playerHand = new List<string>();
        dealerHand = new List<string>();
    }

    public bool Main(string dataFolder)
    {
        this.dataFolder = dataFolder;
        Console.WriteLine("Starting Blackjack Game...");
        InitializeDeck();
        ShuffleDeck();
        DealInitialCards();
        PlayPlayerTurn();
        if (CalculateHandValue(playerHand) <= 21)
        {
            PlayDealerTurn();
        }
        DetermineWinner();
        SaveGameResult();
        return true;
    }

    private void InitializeDeck()
    {
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };
        deck = new List<string>();
        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                deck.Add(rank + " of " + suit);
            }
        }
    }

    private void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            var temp = deck[i];
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
        Console.WriteLine("Your hand: " + string.Join(", ", playerHand) + " (Total: " + CalculateHandValue(playerHand) + ")");
        Console.WriteLine("Dealer's hand: " + dealerHand[0] + ", [Hidden]");
    }

    private string DrawCard()
    {
        if (deck.Count == 0)
        {
            InitializeDeck();
            ShuffleDeck();
        }
        var card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    private int CalculateHandValue(List<string> hand)
    {
        int value = 0;
        int aces = 0;
        foreach (var card in hand)
        {
            string rank = card.Split(' ')[0];
            if (rank == "Jack" || rank == "Queen" || rank == "King")
            {
                value += 10;
            }
            else if (rank == "Ace")
            {
                value += 11;
                aces++;
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

    private void PlayPlayerTurn()
    {
        while (true)
        {
            Console.WriteLine("Do you want to Hit (H) or Stand (S)?");
            string input = Console.ReadLine().Trim().ToUpper();
            if (input == "H")
            {
                playerHand.Add(DrawCard());
                Console.WriteLine("Your hand: " + string.Join(", ", playerHand) + " (Total: " + CalculateHandValue(playerHand) + ")");
                if (CalculateHandValue(playerHand) > 21)
                {
                    Console.WriteLine("Bust! You went over 21.");
                    break;
                }
            }
            else if (input == "S")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter H or S.");
            }
        }
    }

    private void PlayDealerTurn()
    {
        Console.WriteLine("Dealer's hand: " + string.Join(", ", dealerHand) + " (Total: " + CalculateHandValue(dealerHand) + ")");
        while (CalculateHandValue(dealerHand) < 17)
        {
            dealerHand.Add(DrawCard());
            Console.WriteLine("Dealer hits: " + dealerHand[dealerHand.Count - 1]);
            Console.WriteLine("Dealer's hand: " + string.Join(", ", dealerHand) + " (Total: " + CalculateHandValue(dealerHand) + ")");
        }
        if (CalculateHandValue(dealerHand) > 21)
        {
            Console.WriteLine("Dealer busts!");
        }
    }

    private void DetermineWinner()
    {
        int playerValue = CalculateHandValue(playerHand);
        int dealerValue = CalculateHandValue(dealerHand);
        Console.WriteLine("Your total: " + playerValue);
        Console.WriteLine("Dealer's total: " + dealerValue);
        if (playerValue > 21)
        {
            Console.WriteLine("You busted. Dealer wins!");
        }
        else if (dealerValue > 21)
        {
            Console.WriteLine("Dealer busted. You win!");
        }
        else if (playerValue > dealerValue)
        {
            Console.WriteLine("You win!");
        }
        else if (playerValue < dealerValue)
        {
            Console.WriteLine("Dealer wins!");
        }
        else
        {
            Console.WriteLine("It's a tie!");
        }
    }

    private void SaveGameResult()
    {
        try
        {
            string resultPath = Path.Combine(dataFolder, "blackjack_result.json");
            var result = new
            {
                PlayerHand = playerHand,
                DealerHand = dealerHand,
                PlayerTotal = CalculateHandValue(playerHand),
                DealerTotal = CalculateHandValue(dealerHand),
                Timestamp = DateTime.Now
            };
            string json = JsonSerializer.Serialize(result);
            File.WriteAllText(resultPath, json);
            Console.WriteLine("Game result saved to " + resultPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving game result: " + ex.Message);
        }
    }
}