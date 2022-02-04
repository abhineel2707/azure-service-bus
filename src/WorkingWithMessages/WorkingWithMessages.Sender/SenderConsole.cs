using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WorkingWithMessages.MessageEntities;

namespace WorkingWithMessages.Sender
{
    class SenderConsole
    {
        private static string _connectionString;
        private static string _queueName;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Sender Console - Hit Enter", ConsoleColor.White);
            Console.ReadLine();

            var azureConfig = GetAzureConfig();

            _connectionString = azureConfig["connectionString"];
            _queueName = azureConfig["queueName"];

            //await SendTextString("The quick brown fox jumps over the lazy dog.");

            //await SendPizzaOrderAsync();

            //await SendPizzaOrderAsListAsync();

            await SendPizzaOrderListAsBatchAsync();
        }

        private static async Task SendTextString(string text)
        {
            WriteLine("SendTextStringAsMessageAsync", ConsoleColor.Cyan);

            // Create a client
            var client = new QueueClient(_connectionString, _queueName);

            Write("Sending...", ConsoleColor.Green);

            var message = new Message(Encoding.UTF8.GetBytes(text));
            await client.SendAsync(message);

            WriteLine("Done!", ConsoleColor.Green);

            // Close the client
            await client.CloseAsync();
        }

        private static async Task SendPizzaOrderAsync()
        {
            WriteLine("SendPizzaOrderAsync", ConsoleColor.Cyan);

            var order = new PizzaOrder()
            {
                CustomerName = "Abhineel Rai",
                Type = "Hawaiian",
                Size = "Large"
            };

            // Serialize the Object
            var jsonPizzaOrder = JsonConvert.SerializeObject(order);

            // Create a message
            var message = new Message(Encoding.UTF8.GetBytes(jsonPizzaOrder))
            {
                Label = "PizzaOrder",
                ContentType = "application/json"
            };

            // Send the message
            var queueClient = new QueueClient(_connectionString, _queueName);
            Write("Sending order...", ConsoleColor.Green);
            await queueClient.SendAsync(message);

            WriteLine("Done!", ConsoleColor.Green);
            Console.WriteLine();
            await queueClient.CloseAsync();
        }

        private static async Task SendPizzaOrderAsListAsync()
        {
            WriteLine("SendPizzaOrderAsListAsync", ConsoleColor.Cyan);

            var pizzaOrderList = GetPizzaOrderList();

            var queueClient = new QueueClient(_connectionString, _queueName);

            WriteLine("Sending...", ConsoleColor.Yellow);

            var watch = Stopwatch.StartNew();

            foreach (var pizzaOrder in pizzaOrderList)
            {
                var jsonPizzaOrder = JsonConvert.SerializeObject(pizzaOrder);
                var message = new Message(Encoding.UTF8.GetBytes(jsonPizzaOrder))
                {
                    Label = "PizzaOrder",
                    ContentType = "application/json"
                };
                await queueClient.SendAsync(message);
            }
            await queueClient.CloseAsync();
            WriteLine($"Sent {pizzaOrderList.Count} orders! Time: {watch.ElapsedMilliseconds} ms, i.e. {pizzaOrderList.Count / watch.Elapsed.TotalSeconds} message per second", ConsoleColor.Green);
            Console.WriteLine();
            Console.WriteLine();
        }

        private static async Task SendPizzaOrderListAsBatchAsync()
        {
            WriteLine("SendPizzaOrderListAsBatchAsync", ConsoleColor.Cyan);

            var pizzaOrderList = GetPizzaOrderList();

            var queueClient = new QueueClient(_connectionString, _queueName);

            var watch = Stopwatch.StartNew();
            var messageList = new List<Message>();

            foreach (var pizzaOrder in pizzaOrderList)
            {
                var jsonPizzaOrder = JsonConvert.SerializeObject(pizzaOrder);
                var message = new Message(Encoding.UTF8.GetBytes(jsonPizzaOrder))
                {
                    Label = "PizzaOrder",
                    ContentType = "application/json"
                };
                messageList.Add(message);
            }

            WriteLine("Sending...", ConsoleColor.Yellow);
            await queueClient.SendAsync(messageList);

            await queueClient.CloseAsync();

            WriteLine($"Sent {pizzaOrderList.Count} orders! Time: {watch.ElapsedMilliseconds} ms, i.e. {pizzaOrderList.Count / watch.Elapsed.Seconds} message per second", ConsoleColor.Green);
            Console.WriteLine();
            Console.WriteLine();
        }

        private static List<PizzaOrder> GetPizzaOrderList()
        {
            var pizzaOrderList = new List<PizzaOrder>();
            string[] names = { "Jon", "Ron", "Steve" };
            string[] pizzas = { "Farmhouse", "Veggie Delight", "Peppy Paneer", "Indi Tandoori" };

            for (int pizza = 0; pizza < pizzas.Length; pizza++)
            {
                for (int name = 0; name < names.Length; name++)
                {
                    PizzaOrder order = new PizzaOrder()
                    {
                        CustomerName = names[name],
                        Type = pizzas[pizza],
                        Size = "Large"
                    };
                    pizzaOrderList.Add(order);
                }
            }
            return pizzaOrderList;
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
