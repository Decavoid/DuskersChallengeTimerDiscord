using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Discord
{
    private static readonly HttpClient client = new HttpClient();

    private readonly string webHookUrl;

    public Discord(string webHookUrl)
    {
        if (string.IsNullOrEmpty(webHookUrl))
            throw new ArgumentException("webHookUrl");
        this.webHookUrl = webHookUrl;
    }

    public async Task Post(JObject json)
    {
        var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(webHookUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Discord response: " + responseString);
    }
}
