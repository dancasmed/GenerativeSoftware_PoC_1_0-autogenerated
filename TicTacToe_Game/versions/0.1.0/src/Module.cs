using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class TicTacToeModule : IGeneratedModule
{
    public string Name { get; set; } = "TicTacToe Game";
    
    private char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private char playerSymbol = 'X';
    private char computerSymbol = 'O';
    private Random random = new Random();
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Tic-Tac-Toe Game Module");
        Console.WriteLine("You are X, computer is O");
        
        bool gameOver = false;
        bool isPlayerTurn = true;
        
        while (!gameOver)
        {
            PrintBoard();
            
            if (isPlayerTurn)
            {
                PlayerMove();
            }
            else
            {
                Console.WriteLine("Computer's turn...");
                ComputerMove();
            }
            
            if (CheckWinner() != ' ')
            {
                PrintBoard();
                char winner = CheckWinner();
                if (winner == playerSymbol)
                {
                    Console.WriteLine("Congratulations! You won!");
                }
                else
                {
                    Console.WriteLine("Computer wins! Better luck next time.");
                }
                gameOver = true;
            }
            else if (IsBoardFull())
            {
                PrintBoard();
                Console.WriteLine("It's a draw!");
                gameOver = true;
            }
            
            isPlayerTurn = !isPlayerTurn;
        }
        
        SaveGameResult(dataFolder);
        return true;
    }
    
    private void PrintBoard()
    {
        Console.WriteLine(" " + board[0] + " | " + board[1] + " | " + board[2]);
        Console.WriteLine("-----------");
        Console.WriteLine(" " + board[3] + " | " + board[4] + " | " + board[5]);
        Console.WriteLine("-----------");
        Console.WriteLine(" " + board[6] + " | " + board[7] + " | " + board[8]);
    }
    
    private void PlayerMove()
    {
        bool validMove = false;
        
        while (!validMove)
        {
            Console.Write("Enter your move (1-9): ");
            string input = Console.ReadLine();
            
            if (int.TryParse(input, out int position) && position >= 1 && position <= 9)
            {
                if (board[position - 1] != playerSymbol && board[position - 1] != computerSymbol)
                {
                    board[position - 1] = playerSymbol;
                    validMove = true;
                }
                else
                {
                    Console.WriteLine("That position is already taken. Try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 9.");
            }
        }
    }
    
    private void ComputerMove()
    {
        bool validMove = false;
        
        while (!validMove)
        {
            int position = random.Next(0, 9);
            
            if (board[position] != playerSymbol && board[position] != computerSymbol)
            {
                board[position] = computerSymbol;
                validMove = true;
            }
        }
    }
    
    private char CheckWinner()
    {
        // Check rows
        for (int i = 0; i < 9; i += 3)
        {
            if (board[i] == board[i + 1] && board[i + 1] == board[i + 2])
            {
                return board[i];
            }
        }
        
        // Check columns
        for (int i = 0; i < 3; i++)
        {
            if (board[i] == board[i + 3] && board[i + 3] == board[i + 6])
            {
                return board[i];
            }
        }
        
        // Check diagonals
        if (board[0] == board[4] && board[4] == board[8])
        {
            return board[0];
        }
        
        if (board[2] == board[4] && board[4] == board[6])
        {
            return board[2];
        }
        
        return ' ';
    }
    
    private bool IsBoardFull()
    {
        foreach (char spot in board)
        {
            if (spot != playerSymbol && spot != computerSymbol)
            {
                return false;
            }
        }
        return true;
    }
    
    private void SaveGameResult(string dataFolder)
    {
        try
        {
            string result = CheckWinner() == playerSymbol ? "Player Win" : 
                           CheckWinner() == computerSymbol ? "Computer Win" : "Draw";
            
            var gameResult = new
            {
                Date = DateTime.Now,
                Result = result,
                Board = string.Join(",", board)
            };
            
            string fileName = Path.Combine(dataFolder, "tictactoe_results.json");
            string jsonString = JsonSerializer.Serialize(gameResult);
            
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            
            File.AppendAllText(fileName, jsonString + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving game result: " + ex.Message);
        }
    }
}