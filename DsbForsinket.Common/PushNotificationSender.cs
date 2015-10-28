using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.NotificationHubs;

namespace DsbForsinket.Common
{
    public class PushNotificationSender
    {
        private readonly Action<string> log;
        private readonly NotificationHubClient hubClient;

        public PushNotificationSender(Action<string> log)
        {
            this.log = log;
            string notificationHubName = ConfigurationManager.AppSettings["MS_NotificationHubName"];
            string notificationHubConnection = ConfigurationManager.ConnectionStrings["MS_NotificationHubConnectionString"].ConnectionString;

            this.hubClient = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);
        }

        public async Task<NotificationOutcome> SendAsync(string message)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["message"] = message
            };

            return await this.SendAsync(data);
        }

        public async Task<NotificationOutcome> SendAsync(Dictionary<string, string> data)
        {
            return await hubClient.SendGcmNativeNotificationAsync(new GooglePushMessage(data, TimeSpan.Zero).ToString());
        }

        public async Task<NotificationOutcome> SendAsync(string message, string tag)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["message"] = message
            };

            return await this.SendAsync(data, tag);
        }

        public async Task<NotificationOutcome> SendAsync(Dictionary<string, string> data, string tag)
        {
            var jsonPayload = new GooglePushMessage(data, TimeSpan.Zero).ToString();
            this.log(jsonPayload);
            return await hubClient.SendGcmNativeNotificationAsync(jsonPayload, new[] { tag });
        }
    }
}