using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    class SubscriptionReceiver
    {
        private SubscriptionClient subscriptionClient;
        public SubscriptionReceiver(string connectionString,string topicPath,string subscriptionName)
        {
            subscriptionClient = new SubscriptionClient(connectionString, topicPath, subscriptionName);
        }

        public void RegisterMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            subscriptionClient.RegisterMessageHandler(ProcessOrderMessageAsync, messageHandlerOptions);
        }

        private async Task ProcessOrderMessageAsync(Message message, CancellationToken token)
        {
            // Process the order message
            var orderJson = Encoding.UTF8.GetString(message.Body);
            var order = JsonConvert.DeserializeObject<Order>(orderJson);

            Console.WriteLine($"{order.ToString()}");

            // Complete the message
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered and exception {exceptionReceivedEventArgs.Exception}");
            return Task.CompletedTask;
        }

        public async Task Close()
        {
            await subscriptionClient.CloseAsync();
        }
    }
}
