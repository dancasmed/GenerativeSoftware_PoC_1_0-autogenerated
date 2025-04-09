using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ScriptAnalyzerModule : IGeneratedModule
{
    public string Name { get; set; } = "Script Analyzer Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Script Analyzer Module is running...");
        
        try
        {
            string scriptPath = Path.Combine(dataFolder, "script.txt");
            string analysisPath = Path.Combine(dataFolder, "analysis.json");
            
            if (!File.Exists(scriptPath))
            {
                Console.WriteLine("Error: Script file not found in the data folder.");
                return false;
            }
            
            string scriptContent = File.ReadAllText(scriptPath);
            ScriptAnalysis analysis = AnalyzeScript(scriptContent);
            
            string jsonAnalysis = JsonSerializer.Serialize(analysis, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(analysisPath, jsonAnalysis);
            
            Console.WriteLine("Script analysis completed successfully.");
            Console.WriteLine("Analysis saved to: " + analysisPath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during script analysis: " + ex.Message);
            return false;
        }
    }
    
    private ScriptAnalysis AnalyzeScript(string scriptContent)
    {
        ScriptAnalysis analysis = new ScriptAnalysis();
        
        string[] lines = scriptContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            
            if (string.IsNullOrWhiteSpace(trimmedLine))
                continue;
                
            analysis.TotalLines++;
            
            if (trimmedLine.StartsWith("INT.") || trimmedLine.StartsWith("EXT."))
            {
                analysis.Scenes.Add(trimmedLine);
                analysis.SceneCount++;
            }
            else if (trimmedLine.StartsWith("CHARACTER:"))
            {
                string character = trimmedLine.Split(':')[0].Trim();
                if (!analysis.Characters.Contains(character))
                {
                    analysis.Characters.Add(character);
                    analysis.CharacterCount++;
                }
                analysis.DialogueCount++;
            }
            else if (trimmedLine.StartsWith("(") && trimmedLine.EndsWith(")"))
            {
                analysis.ActionCount++;
            }
            else if (trimmedLine.Length > 0 && char.IsUpper(trimmedLine[0]))
            {
                // This might be a character name without the : (some script formats)
                if (!analysis.Characters.Contains(trimmedLine))
                {
                    analysis.Characters.Add(trimmedLine);
                    analysis.CharacterCount++;
                }
            }
        }
        
        return analysis;
    }
}

public class ScriptAnalysis
{
    public int TotalLines { get; set; }
    public int SceneCount { get; set; }
    public int CharacterCount { get; set; }
    public int DialogueCount { get; set; }
    public int ActionCount { get; set; }
    public List<string> Scenes { get; set; } = new List<string>();
    public List<string> Characters { get; set; } = new List<string>();
}