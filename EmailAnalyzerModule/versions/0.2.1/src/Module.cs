#region Using Directives
using System;
using System.IO;
using System.Text;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Rest;
using System.Net.Http;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
#endregion
namespace TextAnalysis
{
    public class Program
    {
        #region Private Fields
        private static readonly string _subscriptionKey = "YOUR_SUBSCRIPTION_KEY";
        private static readonly string _endpoint = "https://YOUR_ENDPOINT.cognitiveservices.azure.com/";
        #endregion
        public static void Main(string[] args)
        {
            var credentials = new ServiceClientCredentials();
            credentials.ApiKey.QueryParameterName = "Ocp-Apim-Subscription-Key";
            credentials.ApiKey.Value = _subscriptionKey;
            var client = new TextAnalyticsClient(credentials)
            {
                Endpoint = _endpoint
            };
            #region Get Entities from text
            var entitiesResult = client.EntitiesAsync("I had a wonderful trip to Seattle last week.").Result;
            foreach (var entity in entitiesResult.Entities)
            {
                Console.WriteLine($"\tName: {entity.Name}, Type: {entity.Type}");
            }
            #endregion
        }
    }
}