﻿using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using RfidCheckout.Config;
using RfidCheckout.Messages;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RfidCheckout.TagReader
{
    internal class TagReaderConsole
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Tag Reader Console");

            QueueClient queueClient = new QueueClient(AccountDetails.ConnectionString,AccountDetails.QueueName);

            // Create a sample order
            RfidTag[] orderItems = new RfidTag[]
            {
                new RfidTag(){Product="Ball",Price=4.99},
                new RfidTag(){Product="Whistle",Price=1.95},
                new RfidTag(){Product="Bat",Price=12.99},
                new RfidTag(){Product="Bat",Price=12.99},
                new RfidTag(){Product="Gloves",Price=7.99},
                new RfidTag(){Product="Gloves",Price=7.99},
                new RfidTag(){Product="Cap",Price=9.99},
                new RfidTag(){Product="Cap",Price=9.99},
                new RfidTag(){Product="Shirt",Price=14.99},
                new RfidTag(){Product="Shirt",Price=14.99}
            };

            // Display order data
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Order contains {0} items.",orderItems.Length);
            Console.ForegroundColor = ConsoleColor.Yellow;

            double orderTotal = 0.0;
            foreach (RfidTag tag in orderItems)
            {
                Console.WriteLine($"{tag.Product} - {tag.Price}");
                orderTotal += tag.Price;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Order value = {orderTotal}");
            Console.WriteLine();
            Console.ResetColor();

            Console.WriteLine("Press enter to scan...");
            
            Console.ReadLine();

            Random random = new Random(DateTime.Now.Millisecond);

            // Send the order with random duplicate tag reads
            int sentCount = 0;
            int position = 0;

            Console.WriteLine("Reading tags...");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;

            // Comment in to create session id
            var sessionId = Guid.NewGuid().ToString();
            Console.WriteLine($"SessionId: { sessionId }");

            while (position < 10)
            {
                RfidTag rfidTag = orderItems[position];

                // Create a new  message from the order item RFID tag.
                var orderJson = JsonConvert.SerializeObject(rfidTag);
                var tagReadMessage = new Message(Encoding.UTF8.GetBytes(orderJson));

                // Comment in to set message id.
                tagReadMessage.MessageId = rfidTag.TagId;

                // Comment in to set session id.
                tagReadMessage.SessionId = sessionId;

                // Send the message
                await queueClient.SendAsync(tagReadMessage);
                //Console.WriteLine($"Sent: { orderItems[position].Product }");
                Console.WriteLine($"Sent: { orderItems[position].Product } - MessageId: { tagReadMessage.MessageId }");

                var randomVal= random.NextDouble();
                // Randomly cause a duplicate message to be sent.
                if (randomVal > 0.4) position++;
                sentCount++;

                Thread.Sleep(100);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} total tag reads.", sentCount);
            Console.WriteLine();
            Console.ResetColor();

            Console.ReadLine();
        }
    }
}
