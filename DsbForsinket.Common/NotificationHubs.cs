using System.Configuration;
using Microsoft.Azure.NotificationHubs;

namespace DsbForsinket.Common
{
    public static class NotificationHubClients
    {
        private static readonly object LockObject = new object();
        private static NotificationHubClient notificationHubClient;

        public static NotificationHubClient Default
        {
            get
            {
                if (notificationHubClient == null)
                {
                    lock(LockObject)
                    if (notificationHubClient == null)
                    {
                        
                    string notificationHubName = ConfigurationManager.AppSettings["MS_NotificationHubName"];
                    string notificationHubConnection = ConfigurationManager.ConnectionStrings["MS_NotificationHubConnectionString"].ConnectionString;
                    notificationHubClient = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);
                    }
                }
                return notificationHubClient;
            }
        }
    }
}
