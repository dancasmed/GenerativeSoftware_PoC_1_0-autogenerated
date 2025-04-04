using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class DungeonCrawlerModule : IGeneratedModule
{
    public string Name { get; set; } = "Dungeon Crawler Module";

    private Random random = new Random();
    private string playerDataPath;
    private string dungeonDataPath;

    public bool Main(string dataFolder)
    {
        playerDataPath = Path.Combine(dataFolder, "player_data.json");
        dungeonDataPath = Path.Combine(dataFolder, "dungeon_data.json");

        Console.WriteLine("Initializing Dungeon Crawler...");

        Player player = LoadPlayerData();
        Dungeon dungeon = LoadDungeonData();

        if (player == null)
        {
            player = new Player { Health = 100, AttackPower = 10, Gold = 0, Inventory = new List<string>() };
            Console.WriteLine("New player created.");
        }

        if (dungeon == null)
        {
            dungeon = GenerateNewDungeon();
            Console.WriteLine("New dungeon generated.");
        }

        bool gameOver = false;
        while (!gameOver && player.Health > 0)
        {
            Console.WriteLine("\nCurrent Room: " + dungeon.CurrentRoom.Description);
            Console.WriteLine("Player Health: " + player.Health + ", Gold: " + player.Gold);

            if (dungeon.CurrentRoom.HasEnemy)
            {
                Enemy enemy = dungeon.CurrentRoom.Enemy;
                Console.WriteLine("You encounter a " + enemy.Name + " with " + enemy.Health + " health!");

                while (enemy.Health > 0 && player.Health > 0)
                {
                    Console.WriteLine("\n1. Attack");
                    Console.WriteLine("2. Try to flee");
                    Console.Write("Choose an action: ");
                    string input = Console.ReadLine();

                    if (input == "1")
                    {
                        int playerDamage = random.Next(5, player.AttackPower + 1);
                        enemy.Health -= playerDamage;
                        Console.WriteLine("You hit the " + enemy.Name + " for " + playerDamage + " damage!");

                        if (enemy.Health > 0)
                        {
                            int enemyDamage = random.Next(2, enemy.AttackPower + 1);
                            player.Health -= enemyDamage;
                            Console.WriteLine("The " + enemy.Name + " hits you for " + enemyDamage + " damage!");
                        }
                        else
                        {
                            Console.WriteLine("You defeated the " + enemy.Name + "!");
                            player.Gold += enemy.GoldReward;
                            dungeon.CurrentRoom.HasEnemy = false;
                            break;
                        }
                    }
                    else if (input == "2")
                    {
                        if (random.Next(0, 2) == 0)
                        {
                            Console.WriteLine("You successfully fled from the " + enemy.Name + "!");
                            dungeon.MoveToPreviousRoom();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("You failed to flee!");
                            int enemyDamage = random.Next(2, enemy.AttackPower + 1);
                            player.Health -= enemyDamage;
                            Console.WriteLine("The " + enemy.Name + " hits you for " + enemyDamage + " damage!");
                        }
                    }
                }

                if (player.Health <= 0)
                {
                    Console.WriteLine("You have been defeated! Game over.");
                    gameOver = true;
                    break;
                }
            }

            if (dungeon.CurrentRoom.HasLoot && !gameOver)
            {
                Loot loot = dungeon.CurrentRoom.Loot;
                Console.WriteLine("You found " + loot.Name + "!");
                Console.WriteLine("1. Take it");
                Console.WriteLine("2. Leave it");
                Console.Write("Choose an action: ");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    player.Inventory.Add(loot.Name);
                    player.Gold += loot.GoldValue;
                    Console.WriteLine("You added " + loot.Name + " to your inventory and gained " + loot.GoldValue + " gold!");
                    dungeon.CurrentRoom.HasLoot = false;
                }
            }

            if (!gameOver)
            {
                Console.WriteLine("\n1. Move to next room");
                if (dungeon.CurrentRoomIndex > 0)
                {
                    Console.WriteLine("2. Move to previous room");
                }
                Console.WriteLine("3. Save and quit");
                Console.Write("Choose an action: ");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    dungeon.MoveToNextRoom();
                }
                else if (input == "2" && dungeon.CurrentRoomIndex > 0)
                {
                    dungeon.MoveToPreviousRoom();
                }
                else if (input == "3")
                {
                    SaveGame(player, dungeon);
                    Console.WriteLine("Game saved. Goodbye!");
                    gameOver = true;
                }
            }
        }

        if (player.Health <= 0)
        {
            Console.WriteLine("Game over. Your score: " + player.Gold);
            File.Delete(playerDataPath);
            File.Delete(dungeonDataPath);
        }

        return true;
    }

    private Player LoadPlayerData()
    {
        if (File.Exists(playerDataPath))
        {
            string json = File.ReadAllText(playerDataPath);
            return JsonSerializer.Deserialize<Player>(json);
        }
        return null;
    }

    private Dungeon LoadDungeonData()
    {
        if (File.Exists(dungeonDataPath))
        {
            string json = File.ReadAllText(dungeonDataPath);
            return JsonSerializer.Deserialize<Dungeon>(json);
        }
        return null;
    }

    private void SaveGame(Player player, Dungeon dungeon)
    {
        string playerJson = JsonSerializer.Serialize(player);
        File.WriteAllText(playerDataPath, playerJson);

        string dungeonJson = JsonSerializer.Serialize(dungeon);
        File.WriteAllText(dungeonDataPath, dungeonJson);
    }

    private Dungeon GenerateNewDungeon()
    {
        var dungeon = new Dungeon();
        dungeon.Rooms = new List<Room>();

        string[] roomDescriptions = {
            "A dark, damp cave with water dripping from the ceiling.",
            "A torch-lit corridor with strange markings on the walls.",
            "A large chamber with bones scattered across the floor.",
            "A narrow tunnel that seems to go on forever.",
            "A circular room with a mysterious altar in the center.",
            "A room filled with ancient, rusted weapons.",
            "A chamber with a deep pit in the middle.",
            "A room with walls covered in glowing mushrooms.",
            "A grand hall with broken chandeliers hanging from the ceiling.",
            "The final chamber with a massive treasure chest."
        };

        string[] enemyNames = { "Goblin", "Skeleton", "Orc", "Spider", "Zombie" };
        string[] lootNames = { "Golden Chalice", "Ancient Sword", "Magic Ring", "Pile of Coins", "Gemstone" };

        for (int i = 0; i < 10; i++)
        {
            var room = new Room
            {
                Description = roomDescriptions[i],
                HasEnemy = random.Next(0, 2) == 0,
                HasLoot = random.Next(0, 2) == 0
            };

            if (room.HasEnemy)
            {
                room.Enemy = new Enemy
                {
                    Name = enemyNames[random.Next(0, enemyNames.Length)],
                    Health = random.Next(15, 31),
                    AttackPower = random.Next(5, 11),
                    GoldReward = random.Next(5, 21)
                };
            }

            if (room.HasLoot)
            {
                room.Loot = new Loot
                {
                    Name = lootNames[random.Next(0, lootNames.Length)],
                    GoldValue = random.Next(10, 51)
                };
            }

            dungeon.Rooms.Add(room);
        }

        return dungeon;
    }
}

public class Player
{
    public int Health { get; set; }
    public int AttackPower { get; set; }
    public int Gold { get; set; }
    public List<string> Inventory { get; set; }
}

public class Dungeon
{
    public List<Room> Rooms { get; set; }
    public int CurrentRoomIndex { get; private set; } = 0;

    public Room CurrentRoom => Rooms[CurrentRoomIndex];

    public void MoveToNextRoom()
    {
        if (CurrentRoomIndex < Rooms.Count - 1)
        {
            CurrentRoomIndex++;
        }
    }

    public void MoveToPreviousRoom()
    {
        if (CurrentRoomIndex > 0)
        {
            CurrentRoomIndex--;
        }
    }
}

public class Room
{
    public string Description { get; set; }
    public bool HasEnemy { get; set; }
    public Enemy Enemy { get; set; }
    public bool HasLoot { get; set; }
    public Loot Loot { get; set; }
}

public class Enemy
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int AttackPower { get; set; }
    public int GoldReward { get; set; }
}

public class Loot
{
    public string Name { get; set; }
    public int GoldValue { get; set; }
}