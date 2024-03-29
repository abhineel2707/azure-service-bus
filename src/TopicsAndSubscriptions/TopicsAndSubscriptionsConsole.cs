﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    class TopicsAndSubscriptionsConsole
    {
        private static string ServiceBusConnectionString = "";
        private static string OrdersTopicPath = "Orders";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Topics And Subscriptions Console!");

            PrompAndWait("Press enter to create topics and subscriptions...");
            await CreateTopicsAndSubscriptions();

            PrompAndWait("Press enter to send order messages...");
            await SendOrderMessages();

            PrompAndWait("Press enter to receive order messages...");
            await ReceiveOrderFromAllSubscriptions();

            PrompAndWait("Topic and Subscription console complete");
        }

        private static async Task CreateTopicsAndSubscriptions()
        {
            var manager = new Manager(ServiceBusConnectionString);
            await manager.CreateTopic(OrdersTopicPath);

            await manager.CreateSubscription(OrdersTopicPath, "AllOrders");

            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "UsaOrders", "region = 'USA'");
            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "EuOrders", "region = 'EU'");

            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "LargeOrders", "items > 30");
            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "HighValueOrders", "value > 500");

            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "LoyalityCardOrders", "loyality = true AND region = 'USA'");

            await manager.CreateSubscriptionWithCorrelationFilter(OrdersTopicPath, "UkOrders", "UK");

        }

        private static async Task SendOrderMessages()
        {
            var orders = CreateTestOrders();

            var sender = new TopicSender(ServiceBusConnectionString, OrdersTopicPath);

            foreach (var order in orders)
            {
                await sender.SendOrderMessage(order);
            }

            await sender.Close();
        }

        private static async Task ReceiveOrderFromAllSubscriptions()
        {
            var manager = new Manager(ServiceBusConnectionString);

            // Get all the subscriptions from our topic
            var subscriptionDescriptions = await manager.GetSubscriptionsForTopic(OrdersTopicPath);

            // Loop through subscriptions and process the order message
            foreach (var subscriptionDescription in subscriptionDescriptions)
            {
                var receiver = new SubscriptionReceiver(ServiceBusConnectionString, OrdersTopicPath, subscriptionDescription.SubscriptionName);
                receiver.RegisterMessageHandler();
                PrompAndWait($"Receiving orders from { subscriptionDescription.SubscriptionName }, press enter when complete..");
                await receiver.Close();
            }
        }

        static List<Order> CreateTestOrders()
        {
            var orders = new List<Order>();

            orders.Add(new Order()
            {
                Name = "Loyal Customer",
                Value = 19.99,
                Region = "USA",
                Items = 1,
                HasLoyalityCard = true
            });
            orders.Add(new Order()
            {
                Name = "Large Order",
                Value = 49.99,
                Region = "USA",
                Items = 50,
                HasLoyalityCard = false
            });
            orders.Add(new Order()
            {
                Name = "High Value",
                Value = 749.45,
                Region = "USA",
                Items = 45,
                HasLoyalityCard = false
            });
            orders.Add(new Order()
            {
                Name = "Loyal Europe",
                Value = 49.45,
                Region = "EU",
                Items = 3,
                HasLoyalityCard = true
            });
            orders.Add(new Order()
            {
                Name = "UK Order",
                Value = 49.45,
                Region = "UK",
                Items = 3,
                HasLoyalityCard = false
            });

            // Feel free to add more orders if you like.


            return orders;
        }

        static void PrompAndWait(string text)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.ForegroundColor = temp;

            Console.ReadLine();
        }
    }
}
