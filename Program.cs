using System;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace SalesforceIntegrationApp
{
    class Program
    {
        // Salesforce API credentials
        private const string ClientId = "your_client_id";
        private const string ClientSecret = "your_client_secret";
        private const string Username = "your_salesforce_username";
        private const string Password = "your_salesforce_password";
        private const string TokenRequestUrl = "https://login.salesforce.com/services/oauth2/token";
        private const string ApexEndpointUrl = "https://your_instance.salesforce.com/services/apexrest/MyCustomEndpoint";

        static async Task Main()
        {
            try
            {
                // Step 1: Authenticate and get the access token
                var accessToken = await GetSalesforceAccessToken();

                // Step 2: Call the Apex REST endpoint
                await CallApexEndpoint(accessToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Method to obtain an OAuth access token from Salesforce
        private static async Task<string> GetSalesforceAccessToken()
        {
            var client = new RestClient(TokenRequestUrl);
            var request = new RestRequest(Method.POST);
            
            request.AddParameter("grant_type", "password");
            request.AddParameter("client_id", ClientId);
            request.AddParameter("client_secret", ClientSecret);
            request.AddParameter("username", Username);
            request.AddParameter("password", Password);

            var response = await client.ExecuteAsync(request);
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Failed to retrieve access token: " + response.Content);
            }

            var jsonResponse = JObject.Parse(response.Content);
            return jsonResponse["access_token"].ToString();
        }

        // Method to call the Apex REST endpoint with the access token
        private static async Task CallApexEndpoint(string accessToken)
        {
            var client = new RestClient(ApexEndpointUrl);
            var request = new RestRequest(Method.GET); // Change to POST, PUT, DELETE as needed
            request.AddHeader("Authorization", "Bearer " + accessToken);
            request.AddHeader("Content-Type", "application/json");

            var response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("Apex endpoint response: " + response.Content);
            }
            else
            {
                throw new Exception("Failed to call Apex endpoint: " + response.Content);
            }
        }
    }
}
