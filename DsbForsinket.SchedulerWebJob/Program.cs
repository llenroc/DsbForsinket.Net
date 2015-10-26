using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using Microsoft.ServiceBus.Messaging;

namespace DsbForsinket.SchedulerWebJob
{
    public class Program
    {
        public static void Main()
        {
            Temp().Wait();
        }

        private static async Task Temp()
        {
            var settings = new ServiceSettingsProvider().GetServiceSettings();
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings.Connections[ServiceSettingsKeys.NotificationHubConnectionString].ConnectionString;

            var hubClient = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            var cphTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Brussels, Copenhagen, Madrid, Paris");

            var minutesRounded = (cphTime.Minute / 15) * 15;
            var timeTag = $"{cphTime.Hour}:{minutesRounded}";

            var stationsTags = new HashSet<string>();
            var StationTagPrefix = "station:";

            string continuationToken = null;
            do
            {
                var queryResult = await hubClient.GetRegistrationsByTagAsync(timeTag, continuationToken, Int32.MaxValue);
                continuationToken = queryResult.ContinuationToken;
                var registeredTags = queryResult
                                    .SelectMany(registration => registration.Tags)
                                    .Where(tag => tag.StartsWith(StationTagPrefix));

                foreach (var tag in registeredTags)
                {
                    stationsTags.Add(tag);
                }
            } while (continuationToken != null);

            var queueConnectionString = settings.Connections["MS_QueueConnectionString"].ConnectionString;
            var queueName = settings["MS_QueueName"];
            QueueClient queueClient = QueueClient.CreateFromConnectionString(queueConnectionString, queueName);  

            var stations = stationsTags.Select(tag => tag.Remove(0, StationTagPrefix.Length));

            foreach (var station in stations)
            {
                var message = new BrokeredMessage();
                message.Properties["station"] = station;
                await queueClient.SendAsync(message);
            }
        }
    }
}
