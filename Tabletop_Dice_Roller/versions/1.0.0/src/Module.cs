using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public class DiceRoll
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DiceType { get; set; }
    public int Quantity { get; set; }
    public int[] Results { get; set; }
    public int Total { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class SavedCombination
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string[] DiceTypes { get; set; }
    public int[] Quantities { get; set; }
}

public class RollHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<DiceRoll> Rolls { get; set; } = new List<DiceRoll>();
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class DiceRollerModule : IGeneratedModule
{
    public string Name { get; set; } = "Dice Roller Module";
    private readonly Random _random = new Random();
    private List<SavedCombination> _savedCombinations = new List<SavedCombination>();
    private List<RollHistory> _rollHistory = new List<RollHistory>();
    private string _combinationsPath;
    private string _historyPath;
    private readonly List<string> _validDice = new List<string> { "d4", "d6", "d8", "d10", "d12", "d20", "d100" };

    private int RollDice(int sides) => _random.Next(1, sides + 1);

    private List<DiceRoll> ExecuteRoll(List<Tuple<string, int>> diceSet)
    {
        var rolls = new List<DiceRoll>();
        foreach (var (diceType, quantity) in diceSet)
        {
            var sides = int.Parse(diceType.Substring(1));
            var results = new int[quantity];
            for (int i = 0; i < quantity; i++)
            {
                results[i] = RollDice(sides);
            }
            rolls.Add(new DiceRoll
            {
                DiceType = diceType,
                Quantity = quantity,
                Results = results,
                Total = results.Sum()
            });
        }
        return rolls;
    }

    private void SaveData<T>(string path, List<T> data)
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
    }

    private List<T> LoadData<T>(string path)
    {
        if (!File.Exists(path)) return new List<T>();
        return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(path)) ?? new List<T>();
    }

    private void ShowRollResults(List<DiceRoll> rolls)
    {
        Console.WriteLine("Roll Results:");
        foreach (var roll in rolls)
        {
            Console.WriteLine("{0}x{1}: {2} | Total: {3}",
                roll.Quantity,
                roll.DiceType,
                string.Join(", ", roll.Results),
                roll.Total);
        }
    }

    private void MainMenu()
    {
        while (true)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Roll Dice");
            Console.WriteLine("2. Save Combination");
            Console.WriteLine("3. Load Combination");
            Console.WriteLine("4. View History");
            Console.WriteLine("5. Exit");
            Console.Write("Select option: ");

            List<Tuple<string, int>> diceSet = null;

            switch (Console.ReadLine())
            {
                case "1":
                    diceSet = new List<Tuple<string, int>>();
                    Console.WriteLine("\nAvailable dice: d4, d6, d8, d10, d12, d20, d100");
                    while (true)
                    {
                        Console.Write("Enter dice type (or 'done'): ");
                        var input = Console.ReadLine()?.ToLower().Trim();
                        if (input == "done") break;

                        if (!_validDice.Contains(input))
                        {
                            Console.WriteLine("Invalid dice type! Valid options: " + string.Join(", ", _validDice));
                            continue;
                        }

                        Console.Write("Quantity: ");
                        if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                        {
                            diceSet.Add(Tuple.Create(input, quantity));
                        }
                        else
                        {
                            Console.WriteLine("Invalid quantity! Must be positive number.");
                        }
                    }
                    if (diceSet.Count > 0)
                    {
                        var result = ExecuteRoll(diceSet);
                        ShowRollResults(result);
                        _rollHistory.Add(new RollHistory { Rolls = result });
                        SaveData(_historyPath, _rollHistory);
                    }
                    break;

                case "2":
                    if (diceSet == null || diceSet.Count == 0)
                    {
                        Console.WriteLine("No valid dice set created. Please use option 1 first.");
                        break;
                    }
                    Console.Write("Combination name: ");
                    var name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        var combo = new SavedCombination
                        {
                            Name = name,
                            DiceTypes = diceSet.Select(d => d.Item1).ToArray(),
                            Quantities = diceSet.Select(d => d.Item2).ToArray()
                        };
                        _savedCombinations.Add(combo);
                        SaveData(_combinationsPath, _savedCombinations);
                    }
                    else
                    {
                        Console.WriteLine("Invalid combination name!");
                    }
                    break;

                case "3":
                    Console.WriteLine("Saved Combinations:");
                    foreach (var c in _savedCombinations)
                    {
                        Console.WriteLine("{0}: {1}", c.Id, c.Name);
                    }
                    Console.Write("Enter combination ID: ");
                    if (Guid.TryParse(Console.ReadLine(), out Guid comboId))
                    {
                        var selected = _savedCombinations.FirstOrDefault(c => c.Id == comboId);
                        if (selected != null)
                        {
                            var comboDiceSet = selected.DiceTypes
                                .Zip(selected.Quantities, (t, q) => Tuple.Create(t, q))
                                .ToList();
                            var comboResult = ExecuteRoll(comboDiceSet);
                            ShowRollResults(comboResult);
                            _rollHistory.Add(new RollHistory { Rolls = comboResult });
                            SaveData(_historyPath, _rollHistory);
                        }
                    }
                    break;

                case "4":
                    Console.WriteLine("\nRoll History:");
                    foreach (var history in _rollHistory)
                    {
                        Console.WriteLine("[{0}] {1} rolls",
                            history.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                            history.Rolls.Count);
                    }
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Dice Roller Module started");
        _combinationsPath = Path.Combine(dataFolder, "combinations.json");
        _historyPath = Path.Combine(dataFolder, "history.json");

        Directory.CreateDirectory(dataFolder);
        _savedCombinations = LoadData<SavedCombination>(_combinationsPath);
        _rollHistory = LoadData<RollHistory>(_historyPath);

        MainMenu();
        return true;
    }
}
