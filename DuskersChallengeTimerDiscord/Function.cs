using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DuskersChallengeTimerDiscord
{
    public class Function
    {
        public enum Command { Invalid, ChallengeWillResetAfterHour, ChallengeResetNow }

        private Settings settings = null;

        public Stream FunctionHandler(Stream input, ILambdaContext context)
        {
            Command command;
            using (StreamReader sr = new StreamReader(input))
            {
                string inputString = sr.ReadToEnd();
                Console.WriteLine("input: " + input);

                if (string.IsNullOrEmpty(inputString))
                    throw new Exception("invalid input");

                command = ParseCommand(inputString);
            }

            var task = FunctionHandlerAsync(command, context);
            task.Wait();
            var result = task.Result;
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            Console.WriteLine("Done.");
            return ms;
        }

        private void LoadSettings()
        {
            if (settings == null)
                settings = new Settings();
        }

        private Command ParseCommand(string s)
        {
            try
            {
                var obj = JObject.Parse(s);
                string command = (string)obj["Cmd"];
                Command cmd = (Command)Enum.Parse(typeof(Command), command);
                return cmd;
            }
            catch (Exception)
            {
                return Command.Invalid;
            }
        }

        public async Task<string> FunctionHandlerAsync(Command command, ILambdaContext context)
        {
            LoadSettings();

            var discord = new Discord(settings.DiscordWebhookUrl);

            Console.WriteLine("Posting to Discord...");
            string content = GetContent(command);

            JObject json = new JObject()
            {
                { "username", "Daily Challenge" },
                { "content", content },
            };

            await discord.Post(json);
            return "OK";
        }

        private string GetContent(Command command)
        {
            switch (command)
            {
                case Command.ChallengeWillResetAfterHour:
                    return GetContentChallengeWillResetAfterHour();
                case Command.ChallengeResetNow:
                    return GetContentChallengeResetNow();
                default:
                    throw new Exception("Invalid command: " + command);
            }
        }

        private string GetContentChallengeWillResetAfterHour()
        {
            var now = DateTime.UtcNow;
            int hoursRemaining = TimeUtils.GetHoursRemainingUntilUTCMidnight();
            string ending = hoursRemaining > 1 ? "s" : string.Empty;
            string msg = string.Format("The Daily will reset in {0} hour{1}!", hoursRemaining, ending);
            return FormatMessage(msg);
        }

        private string GetContentChallengeResetNow()
        {
            string dateString = TimeUtils.GetUtcDateString();
            string msg = string.Format("The Daily has reset! Duskers date is {0}.", dateString);
            return FormatMessage(msg);
        }

        private static string FormatMessage(string msg)
        {
            return "```" + msg + "```";
        }
    }
}
