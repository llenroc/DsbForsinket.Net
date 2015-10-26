using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;

namespace DsbForsinket.Common
{
    public class PushNotificationSender
    {
        private readonly NotificationHubClient hubClient;

        public PushNotificationSender()
        {
            var settings = new ServiceSettingsProvider().GetServiceSettings();
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings.Connections[ServiceSettingsKeys.NotificationHubConnectionString].ConnectionString;

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
            return await hubClient.SendGcmNativeNotificationAsync(new GooglePushMessage(data, TimeSpan.Zero).ToString(), new string[] { tag });
        }
    }
}