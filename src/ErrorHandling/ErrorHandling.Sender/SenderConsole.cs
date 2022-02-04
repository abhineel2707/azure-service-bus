using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;
using ErrorHandling.Config;
using System.Threading;

namespace ErrorHandling.Sender
{
    internal class SenderConsole
    {
        private static QueueClient QueueClient;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Sender Console");
            Console.WriteLine();

            Thread.Sleep(3000);

            QueueClient = new QueueClient(Settings.ConnectionString, Settings.QueuePath);

            while (true)
            {
                Console.WriteLine("text/json/poison/unknown/exit?");

                var messageType = Console.ReadLine().ToLower();

                if (messageType == "exit")
                {
                    break;
                }

                switch (messageType)
                {
                    case "text":
                        await SendMessage("Hello", "text/plain");
                        break;
                    case "json":
                        await SendMessage("{\"contact\": {\"name\": \"Alan\",\"twitter\": \"@alansmith\" }}", "application/json");
                        break;
                    case "poison":
                        await SendMessage("<contact><name>Alan</name><twitter>@alansmith</twitter></contact>", "application/json");
                        break;
                    case "unknown":
                        await SendMessage("Unknown message", "application/unknown");
                        break;

                    default:
                        Console.WriteLine("What?");
                        break;
                }
            }

            await QueueClient.CloseAsync();
        }

        static async Task SendMessage(string text, string contentType)
        {
            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(text));
                message.ContentType = contentType;
                Utils.WriteLine($"Created Message: { text }", ConsoleColor.Cyan);

                await QueueClient.SendAsync(message);
                Utils.WriteLine("Sent Message", ConsoleColor.Cyan);
            }
            catch (Exception ex)
            {
                Utils.WriteLine(ex.Message, ConsoleColor.Yellow);
            }
        }
    }
}
