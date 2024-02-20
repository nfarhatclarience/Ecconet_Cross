using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace SimpleHttpClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string serverUrl = "http://127.0.0.1:8000";  // Replace with your  actual server address
            var canData = new  {
                direction = "IN", 
                timestamp = DateTime.Now.ToString(),  // Simplified timestamp
                id = "ABCD1234",
                data = "FF AA 00 11" // Just some placeholder data 
            };

            string jsonData = JsonConvert.SerializeObject(canData);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverUrl); 
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/can", content); 

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data sent successfully.");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Server Response: " + responseContent);
                }
                else
                {
                    Console.Error.WriteLine("Error sending data: " + response.StatusCode); 
                }
            }
        }
    }
}
