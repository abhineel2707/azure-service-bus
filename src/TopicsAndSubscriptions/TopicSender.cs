using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    class TopicSender
    {
        private TopicClient topicClient;
        public TopicSender(string connectionString,string topicPath)
        {
            topicClient = new TopicClient(connectionString, topicPath);
        }

        public async Task SendOrderMessage(Order order)
        {
            Console.WriteLine($"{order.ToString()}");

            // Serialize order to JSON
            var orderJson = JsonConvert.SerializeObject(order);

            // Create a message containing serialized JSON order
            var message = new Message(Encoding.UTF8.GetBytes(orderJson));

            // Promote Properties
            message.UserProperties.Add("region", order.Region);
            message.UserProperties.Add("items", order.Items);
            message.UserProperties.Add("value", order.Value);
            message.UserProperties.Add("loyality", order.HasLoyalityCard);

            // Set the correlation id
            message.CorrelationId = order.Region;

            // Send the message
            await topicClient.SendAsync(message);
        }

        public async Task Close()
        {
            await topicClient.CloseAsync();
        }
    }
}
