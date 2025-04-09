using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RPGBattleModule : IGeneratedModule
{
    public string Name { get; set; } = "RPG Battle Simulator";

    private string _dataFolder;
    private Random _random = new Random();

    public bool Main(string dataFolder)
    {
        _dataFolder = dataFolder;
        Console.WriteLine("Starting RPG Battle Simulator...");
        Console.WriteLine("Loading game data...");

        try
        {
            InitializeGameData();
            var player = LoadOrCreatePlayer();
            var enemy = GenerateRandomEnemy();

            Console.WriteLine("A wild " + enemy.Name + " appears!");
            Console.WriteLine("Battle begins!");

            bool playerWon = StartBattle(player, enemy);

            if (playerWon)
            {
                Console.WriteLine("You defeated the " + enemy.Name + "!");
                player.Experience += enemy.ExperienceReward;
                Console.WriteLine("Gained " + enemy.ExperienceReward + " experience points!");
            }
            else
            {
                Console.WriteLine("You were defeated by the " + enemy.Name + "!");
            }

            SavePlayerData(player);
            return playerWon;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void InitializeGameData()
    {
        string enemiesPath = Path.Combine(_dataFolder, "enemies.json");
        if (!File.Exists(enemiesPath))
        {
            var defaultEnemies = new List<Enemy>
            {
                new Enemy { Id = 1, Name = "Goblin", Health = 30, Attack = 5, Defense = 2, ExperienceReward = 10 },
                new Enemy { Id = 2, Name = "Orc", Health = 50, Attack = 8, Defense = 4, ExperienceReward = 20 },
                new Enemy { Id = 3, Name = "Dragon", Health = 100, Attack = 15, Defense = 10, ExperienceReward = 50 }
            };

            File.WriteAllText(enemiesPath, JsonSerializer.Serialize(defaultEnemies));
        }
    }

    private Player LoadOrCreatePlayer()
    {
        string playerPath = Path.Combine(_dataFolder, "player.json");
        if (File.Exists(playerPath))
        {
            return JsonSerializer.Deserialize<Player>(File.ReadAllText(playerPath));
        }

        var newPlayer = new Player
        {
            Name = "Hero",
            Health = 100,
            MaxHealth = 100,
            Attack = 10,
            Defense = 5,
            Experience = 0,
            Level = 1
        };

        File.WriteAllText(playerPath, JsonSerializer.Serialize(newPlayer));
        return newPlayer;
    }

    private void SavePlayerData(Player player)
    {
        string playerPath = Path.Combine(_dataFolder, "player.json");
        File.WriteAllText(playerPath, JsonSerializer.Serialize(player));
    }

    private Enemy GenerateRandomEnemy()
    {
        string enemiesPath = Path.Combine(_dataFolder, "enemies.json");
        var enemies = JsonSerializer.Deserialize<List<Enemy>>(File.ReadAllText(enemiesPath));
        return enemies[_random.Next(enemies.Count)];
    }

    private bool StartBattle(Player player, Enemy enemy)
    {
        while (player.Health > 0 && enemy.Health > 0)
        {
            // Player's turn
            int playerDamage = CalculateDamage(player.Attack, enemy.Defense);
            enemy.Health -= playerDamage;
            Console.WriteLine("You hit the " + enemy.Name + " for " + playerDamage + " damage!");

            if (enemy.Health <= 0)
                break;

            // Enemy's turn
            int enemyDamage = CalculateDamage(enemy.Attack, player.Defense);
            player.Health -= enemyDamage;
            Console.WriteLine("The " + enemy.Name + " hits you for " + enemyDamage + " damage!");

            Console.WriteLine("Player HP: " + player.Health + "/" + player.MaxHealth);
            Console.WriteLine("Enemy HP: " + enemy.Health + "/" + enemy.MaxHealth);
            Console.WriteLine();
        }

        return player.Health > 0;
    }

    private int CalculateDamage(int attack, int defense)
    {
        int baseDamage = attack - (defense / 2);
        if (baseDamage < 1) baseDamage = 1;
        return baseDamage + _random.Next(0, 3);
    }
}

public class Player
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Experience { get; set; }
    public int Level { get; set; }
}

public class Enemy
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int ExperienceReward { get; set; }
}