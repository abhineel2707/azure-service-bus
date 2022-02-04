using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkingWithMessages.MessageEntities;

namespace WorkingWithMessages.Receiver
{
    class ReceiverConsole
    {
        private static string _connectionString;
        private static string _queueName;
        private static QueueClient QueueClient;

        static void Main(string[] args)
        {
            Console.WriteLine("Receiver Console", ConsoleColor.White);
            Console.ReadLine();

            var azureConfig = GetAzureConfig();

            _connectionString = azureConfig["connectionString"];
            _queueName = azureConfig["queueName"];

            //ReceiveAndProcessText(1);
            //ReceiveAndProcessPizzaOrder(1);
            ReceiveAndProcessPizzaOrder(5);
        }       

        private static void ReceiveAndProcessText(int threads)
        {
            WriteLine($"ReceiveAndProcessText({threads})",ConsoleColor.Cyan);

            // Create a new queue client
            QueueClient = new QueueClient(_connectionString, _queueName);

            var options = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = threads,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(10)
            };

            // Create a message pump using RegisterMessageHandler
            QueueClient.RegisterMessageHandler(ProcessTextMessageAsync, options);

            WriteLine("Receiving, hit enter to exit",ConsoleColor.White);
            Console.ReadLine();
            StopReceivingAsync().Wait();
        }        

        private static async Task ProcessTextMessageAsync(Message message, CancellationToken token)
        {
            // Deserialize message body
            var messageBodyText = Encoding.UTF8.GetString(message.Body);

            WriteLine($"Received: {messageBodyText}", ConsoleColor.Green);

            // Complete the message
            await QueueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static void ReceiveAndProcessPizzaOrder(int threads)
        {
            WriteLine($"ReceiveAndProcessPizzaOrder({threads})", ConsoleColor.Cyan);

            QueueClient = new QueueClient(_connectionString, _queueName);

            // Set the options for message handler
            var options = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = threads,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(10)
            };

            // Create a message pump
            QueueClient.RegisterMessageHandler(ProcessPizzaOrderAsync, options);

            WriteLine($"Receiving, hit enter to exit", ConsoleColor.White);
            Console.ReadLine();
            StopReceivingAsync().Wait();
        }

        private static async Task ProcessPizzaOrderAsync(Message message, CancellationToken token)
        {
            var messageBodyText = Encoding.UTF8.GetString(message.Body);

            var pizzaOrder = JsonConvert.DeserializeObject<PizzaOrder>(messageBodyText);

            // Process the message
            CookPizza(pizzaOrder);

            await QueueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static void CookPizza(PizzaOrder pizzaOrder)
        {
            WriteLine($"Cooking {pizzaOrder.Type} pizza for {pizzaOrder.CustomerName}.", ConsoleColor.Yellow);
            Thread.Sleep(5000);
            WriteLine($"    {pizzaOrder.Type} pizza for {pizzaOrder.CustomerName} is ready.", ConsoleColor.Green);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            WriteLine(arg.Exception.Message,ConsoleColor.Red);
            return Task.CompletedTask;
        }

        private static async Task StopReceivingAsync()
        {
            // Close the client, which will stop the message pump.
            await QueueClient.CloseAsync();
        }

        private static IConfigurationSection GetAzureConfig()
        {
            var azureConfig = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
               .Build()
               .GetSection("AzureConfig");

            return azureConfig;
        }

        private static void WriteLine(string text, ConsoleColor color)
        {
            var tempColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = tempColor;
        }

        private static void Write(string text, ConsoleColor color)
        {
            var tempColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = tempColor;
        }
    }
}
