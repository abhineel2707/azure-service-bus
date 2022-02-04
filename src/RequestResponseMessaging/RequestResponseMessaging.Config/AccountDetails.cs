using System;

namespace RequestResponseMessaging.Config
{
    public class AccountDetails
    {
        public static string ConnectionString = "Endpoint=sb://abhineeldemos01.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=sPsPUMVByYTC/MPMeCiPi3LiGp+PBVqnwnprdeEwf74=";
        public static string RequestQueueName = "requestqueue";
        public static string ResponseQueueName = "responsequeue";
    }
}
