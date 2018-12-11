using Newtonsoft.Json.Linq;
using System;
using System.IO;

public class Settings
{
    public Settings()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Config", "config.json");
        JObject obj = JObject.Parse(File.ReadAllText(path));
        DiscordWebhookUrl = (string)obj["WebhookUrl"];
    }

    public string DiscordWebhookUrl { get; private set; }
}
