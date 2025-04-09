using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class CaesarCipherModule : IGeneratedModule
{
    public string Name { get; set; } = "Caesar Cipher Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Caesar Cipher Module is running.");

        string settingsPath = Path.Combine(dataFolder, "caesar_settings.json");
        CaesarSettings settings;

        if (File.Exists(settingsPath))
        {
            string json = File.ReadAllText(settingsPath);
            settings = JsonSerializer.Deserialize<CaesarSettings>(json);
        }
        else
        {
            settings = new CaesarSettings { Shift = 3 };
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllText(settingsPath, json);
        }

        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Encrypt message");
            Console.WriteLine("2. Decrypt message");
            Console.WriteLine("3. Change shift value (current: " + settings.Shift + ")");
            Console.WriteLine("4. Exit module");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            if (!int.TryParse(input, out int option))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            switch (option)
            {
                case 1:
                    Console.Write("Enter message to encrypt: ");
                    string messageToEncrypt = Console.ReadLine();
                    string encrypted = Encrypt(messageToEncrypt, settings.Shift);
                    Console.WriteLine("Encrypted message: " + encrypted);
                    break;

                case 2:
                    Console.Write("Enter message to decrypt: ");
                    string messageToDecrypt = Console.ReadLine();
                    string decrypted = Decrypt(messageToDecrypt, settings.Shift);
                    Console.WriteLine("Decrypted message: " + decrypted);
                    break;

                case 3:
                    Console.Write("Enter new shift value: ");
                    if (int.TryParse(Console.ReadLine(), out int newShift))
                    {
                        settings.Shift = newShift;
                        string json = JsonSerializer.Serialize(settings);
                        File.WriteAllText(settingsPath, json);
                        Console.WriteLine("Shift value updated to " + newShift);
                    }
                    else
                    {
                        Console.WriteLine("Invalid shift value.");
                    }
                    break;

                case 4:
                    Console.WriteLine("Exiting Caesar Cipher Module.");
                    return true;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private string Encrypt(string input, int shift)
    {
        return ProcessText(input, shift);
    }

    private string Decrypt(string input, int shift)
    {
        return ProcessText(input, -shift);
    }

    private string ProcessText(string input, int shift)
    {
        shift = shift % 26;
        if (shift < 0)
        {
            shift += 26;
        }

        char[] buffer = input.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char c = buffer[i];
            if (char.IsLetter(c))
            {
                char offset = char.IsUpper(c) ? 'A' : 'a';
                buffer[i] = (char)(((c + shift - offset) % 26) + offset);
            }
        }
        return new string(buffer);
    }

    private class CaesarSettings
    {
        public int Shift { get; set; }
    }
}