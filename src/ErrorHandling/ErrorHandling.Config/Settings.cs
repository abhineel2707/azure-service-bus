using System;

namespace ErrorHandling.Config
{
    public class Settings
    {
        public static string ConnectionString = "";
        public static string QueuePath = "errorhandling";
        public static string ForwardingQueuePath = "errorhandlingforward";
    }
}
