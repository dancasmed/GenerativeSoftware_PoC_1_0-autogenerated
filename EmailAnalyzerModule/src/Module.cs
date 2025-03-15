namespace GenerativeSoftware.Interfaces;
using System;
using System.IO;
using System.Text;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Newtonsoft.Json;
using System.Diagnostics;

public class EmailAnalyzer : IGeneratedModule
{
    public string Name { get; set; }
    private TextAnalyticsClient _textAnalyticsClient;
    private Stopwatch _stopwatch;

    public EmailAnalyzer(string apiKey, string apiEndpoint)
    { 
        var credentials = new ApiKeyServiceClientCredentials(apiKey);
        _textAnalyticsClient = new TextAnalyticsClient(credentials) { Endpoint = apiEndpoint };
        _stopwatch = new Stopwatch();
    }

    public bool Main(string dataFolder)
    { 
        try
        { 
            var files = Directory.GetFiles(dataFolder, "*.eml");
            foreach (var file in files)
            { 
                var emailContent = File.ReadAllText(file);
                _stopwatch.Start();
                var analysisResult = AnalyzeEmail(emailContent);
                _stopwatch.Stop();
                if (analysisResult != null)
                { 
                    var response = GenerateResponse(analysisResult.Intent);
                    var editResponse = EditResponse(response);
                    SaveResponse(editResponse, file, dataFolder);
                    SaveStatistics(_stopwatch.ElapsedMilliseconds, dataFolder);
                }
            }
        } catch (Exception ex)
        { 
            File.WriteAllText(Path.Combine(dataFolder, "error.log"), ex.ToString());
            return false;
        }
        return true;
    }

    private EmailAnalysisResult AnalyzeEmail(string emailContent)
    { 
        var result = _textAnalyticsClient.SentimentAsync(emailContent).Result;
        var intentResult = _textAnalyticsClient.EntityRecognitionAsync(emailContent).Result;
        var intent = GetIntent(intentResult.Entities);
        return new EmailAnalysisResult
        { 
            Sentiment = result.Score,
            Intent = intent
        };
    }

    private string GenerateResponse(string intent)
    { 
        switch (intent)
        { 
            case "reclamo":
                return "Lo sentimos, ¿podría proporcionar más detalles sobre su reclamo?";
            case "consulta":
                return "Estamos aquí para ayudarlo. Por favor, proporcione más información sobre su consulta.";
            case "compra":
                return "Gracias por considerar nuestra empresa. ¿En qué podemos ayudarlo con su compra?";
            default:
                return "Lo sentimos, no entendimos su intención. Por favor, vuelva a intentarlo.";
        }
    }

    private string EditResponse(string response)
    { 
        var editInterface = new ResponseEditor(response);
        return editInterface.GetResponse();
    }

    private void SaveResponse(string response, string emailFile, string dataFolder)
    { 
        var responseFile = Path.Combine(dataFolder, Path.GetFileNameWithoutExtension(emailFile) + ".response.txt");
        File.WriteAllText(responseFile, response);
    }

    private void SaveStatistics(long responseTime, string dataFolder)
    { 
        var statisticsFile = Path.Combine(dataFolder, "statistics.json");
        var statistics = new Statistics
        { 
            ResponseTime = responseTime
        };
        var json = JsonConvert.SerializeObject(statistics);
        File.WriteAllText(statisticsFile, json);
    }

    private string GetIntent(Entity[] entities)
    { 
        foreach (var entity in entities)
        { 
            if (entity.Type == "Intent")
            { 
                return entity.Text;
            }
        }
        return "Desconocido";
    }
}

public class EmailAnalysisResult
{ 
    public double Sentiment { get; set; }
    public string Intent { get; set; }
}

public class ResponseEditor
{ 
    private string _response;

    public ResponseEditor(string response)
    { 
        _response = response;
    }

    public string GetResponse()
    { 
        // Interfaz para editar la respuesta
        return _response;
    }
}

public class Statistics
{ 
    public long ResponseTime { get; set; }
}