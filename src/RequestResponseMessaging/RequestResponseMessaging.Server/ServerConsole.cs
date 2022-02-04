using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using RequestResponseMessaging.Config;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RequestResponseMessaging.Server
{
    internal class ServerConsole
    {
        // Create request and response queue clients
        static QueueClient RequestQueueClient = new QueueClient(AccountDetails.ConnectionString, AccountDetails.RequestQueueName);
        static QueueClient ResponseQueueClient = new QueueClient(AccountDetails.ConnectionString, AccountDetails.ResponseQueueName);

        static async Task Main(string[] args)
        {
            Console.WriteLine("Server Console");

            // Create a management client
            var managementClient = new ManagementClient(AccountDetails.ConnectionString);

            Console.WriteLine("Creating Queues...");

            // Delete any existing queues
            if(await managementClient.QueueExistsAsync(AccountDetails.RequestQueueName))
            {
                await managementClient.DeleteQueueAsync(AccountDetails.RequestQueueName);
            }

            if(await managementClient.QueueExistsAsync(AccountDetails.ResponseQueueName))
            {
                await managementClient.DeleteQueueAsync(AccountDetails.ResponseQueueName);
            }

            await managementClient.CreateQueueAsync(AccountDetails.RequestQueueName);

            QueueDescription queueDescription = new QueueDescription(AccountDetails.ResponseQueueName)
            {
                RequiresSession = true
            };

            await managementClient.CreateQueueAsync(queueDescription);

            Console.WriteLine("Done!");

            RequestQueueClient.RegisterMessageHandler(ProcessRequestMessage, new MessageHandlerOptions(ProcessMessageException));

            Console.WriteLine("Processing, hit enter top exit");
            Console.ReadLine();

            await RequestQueueClient.CloseAsync();
            await ResponseQueueClient.CloseAsync();

        }

        private static async Task ProcessRequestMessage(Message requestMessage, CancellationToken cancellation)
        {
            // Deserialize message body
            string text = Encoding.UTF8.GetString(requestMessage.Body);
            Console.WriteLine($"Received : {text}");

            string echoText = "Echo: " + text;

            // Create a response text using echo text as message body
            var responseMessage=new Message(Encoding.UTF8.GetBytes(echoText));

            // Set the session id
            responseMessage.SessionId = requestMessage.ReplyToSessionId;

            // Send the response message
            await ResponseQueueClient.SendAsync(responseMessage);
            Console.WriteLine($"Sent: {echoText}");
        }

        private static Task ProcessMessageException(ExceptionReceivedEventArgs arg)
        {
            throw new NotImplementedException();
        }
    }
}
