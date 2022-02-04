using Microsoft.Azure.ServiceBus;
using System;
using System.Configuration;
using System.Text;

namespace SimpleBrokeredMessaging.Sender
{
    class SenderConsole
    {
        static void Main(string[] args)
        {
            var queueClient = new QueueClient(ConfigurationManager.AppSettings["accountEndpoint"], ConfigurationManager.AppSettings["queuePath"]);

            // Send messages
            for(int i = 0; i < 10; i++)
            {
                var content = $"Message: {i}";

                var message = new Message(Encoding.UTF8.GetBytes(content));
                queueClient.SendAsync(message).Wait();

                Console.WriteLine("Sent: " + i);
            }

            // Close queue client
            queueClient.CloseAsync().Wait();

            Console.WriteLine("Sent messages...");
            Console.ReadLine();
        }
    }
}
