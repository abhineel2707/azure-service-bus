using Microsoft.Azure.ServiceBus;
using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Receiver
{
    class ReceiverConsole
    {
        static void Main(string[] args)
        {
            var queueClient = new QueueClient(ConfigurationManager.AppSettings["accountEndpoint"], ConfigurationManager.AppSettings["queuePath"]);

            // Create Message Handler to receive messages
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, HandleExceptionsAsync);

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();

            // Close the client
            queueClient.CloseAsync().Wait();
        }
        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var content = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Received: {content}");
        }

        private static Task HandleExceptionsAsync(ExceptionReceivedEventArgs arg)
        {
            throw new NotImplementedException();
        }        
    }
}
