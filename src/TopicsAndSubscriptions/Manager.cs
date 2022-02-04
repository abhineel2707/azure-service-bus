using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    class Manager
    {
        private ManagementClient managementClient;

        public Manager(string connectionString)
        {
            managementClient = new ManagementClient(connectionString);
        }

        public async Task<TopicDescription> CreateTopic(string topicPath)
        {
            Console.WriteLine($"Creating topic {topicPath}");

            if(await managementClient.TopicExistsAsync(topicPath))
            {
                await managementClient.DeleteTopicAsync(topicPath);
            }

            return await managementClient.CreateTopicAsync(topicPath);
        }

        public async Task<SubscriptionDescription> CreateSubscription(string topicPath,string subscriptionName)
        {
            Console.WriteLine($"Creating subscription {topicPath}/{subscriptionName}");

            return await managementClient.CreateSubscriptionAsync(topicPath, subscriptionName);
        }

        public async Task<SubscriptionDescription> CreateSubscriptionWithSqlFilter(string topicPath, string subscriptionName,string sqlExpression)
        {
            Console.WriteLine($"Creating subscription with sql filters {topicPath}/{subscriptionName} ({sqlExpression})");

            var ruleDescription = new RuleDescription("Default", new SqlFilter(sqlExpression));
            var subscriptionDescription = new SubscriptionDescription(topicPath, subscriptionName);

            return await managementClient.CreateSubscriptionAsync(subscriptionDescription, ruleDescription);
        }

        public async Task<SubscriptionDescription> CreateSubscriptionWithCorrelationFilter(string topicPath,string subscriptionName,string correlationId)
        {
            Console.WriteLine($"Creating subscription with correlation id {topicPath}/{subscriptionName} ({correlationId})");

            var ruleDescription = new RuleDescription("Default", new CorrelationFilter(correlationId));
            var subscriptionDescription = new SubscriptionDescription(topicPath, subscriptionName);

            return await managementClient.CreateSubscriptionAsync(subscriptionDescription, ruleDescription);
        }

        public async Task<IList<SubscriptionDescription>> GetSubscriptionsForTopic(string topicPath)
        {
            return await managementClient.GetSubscriptionsAsync(topicPath);
        }
    }
}
