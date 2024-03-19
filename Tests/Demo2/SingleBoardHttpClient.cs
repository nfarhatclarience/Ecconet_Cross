using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

public class SingleBoardHttpClient
{
    private readonly string _serverBaseUrl; 

    public SingleBoardHttpClient(string serverBaseUrl)
    {
        _serverBaseUrl = serverBaseUrl; // Store the server base URL
    }

    public async Task ProcessCanFrame(UInt32 id, byte[] data) 

    {
        string timestamp = DateTime.Now.Second.ToString() + "." + DateTime.Now.Millisecond.ToString();
        string canId = id.ToString("X16");  
        string dataAsHexString = BitConverter.ToString(data).Replace("-", " ");

        var canDataAsJson = JsonConvert.SerializeObject(new {
            direction = "IN", // Or determine dynamically if possible
            timestamp = timestamp, 
            id = canId,
            data = dataAsHexString 
        });



        // Option 2: For Real Use - Transmit over HTTP (add later using HttpClient)
  
        using (var client = new HttpClient())
        {
            var content = new StringContent(canDataAsJson, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json")); // Fix the argument type
            var response = await client.PostAsync($"{_serverBaseUrl}/api/can", content); 

            // Optionally: Check for HTTP success
            if (!response.IsSuccessStatusCode)
            {
                // Handle the error appropriately 
                Console.Error.WriteLine($"Error sending data: {response.StatusCode}"); 
            }
        }
    }
    public async Task SendDeviceInfo(UInt32 id, byte[] data) 
    
    {
        string timestamp = DateTime.Now.Second.ToString() + "." + DateTime.Now.Millisecond.ToString();
        string canId = id.ToString("X16");  
        string dataAsHexString = BitConverter.ToString(data).Replace("-", " ");

        var canDataAsJson = JsonConvert.SerializeObject(new {
            direction = "IN", // Or determine dynamically if possible
            timestamp = timestamp, 
            id = canId,
            data = dataAsHexString 
        });



        // Option 2: For Real Use - Transmit over HTTP (add later using HttpClient)
  
        using (var client = new HttpClient())
        {
            var content = new StringContent(canDataAsJson, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json")); // Fix the argument type
            var response = await client.PostAsync($"{_serverBaseUrl}/api/can", content); 

            // Optionally: Check for HTTP success
            if (!response.IsSuccessStatusCode)
            {
                // Handle the error appropriately 
                Console.Error.WriteLine($"Error sending data: {response.StatusCode}"); 
            }
        }
    }
}