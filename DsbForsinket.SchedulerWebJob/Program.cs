using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.ServiceBus.Messaging;

namespace DsbForsinket.SchedulerWebJob
{
    public class Program
    {
        private const string StationTagPrefix = "station-";
        private const string TimeTagPrefix = "time-";

        public static void Main()
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            string notificationHubName = ConfigurationManager.AppSettings["MS_NotificationHubName"];
            string notificationHubConnection = ConfigurationManager.ConnectionStrings["MS_NotificationHubConnectionString"].ConnectionString;

            var hubClient = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            var cphTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Romance Standard Time");

            // TODO: add some randomly distributed suffix to the time tag to allow more than 10k subscribers
            var minutesRounded = (cphTime.Minute / 15) * 15;
            var timeTag = $"{TimeTagPrefix}{cphTime.Hour}:{minutesRounded}";
            var timeTagNoPrefix = $"{cphTime.Hour}:{minutesRounded}";

            var stationsTags = new HashSet<string>();

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

            string queueName = ConfigurationManager.AppSettings["MS_QueueName"];
            string queueConnectionString = ConfigurationManager.ConnectionStrings["MS_QueueConnectionString"].ConnectionString;

            QueueClient queueClient = QueueClient.CreateFromConnectionString(queueConnectionString, queueName);  

            var stations = stationsTags.Select(tag => tag.Remove(0, StationTagPrefix.Length));

            foreach (var station in stations)
            {
                var message = new BrokeredMessage();
                message.Properties["station"] = station;
                message.Properties["timetag"] = timeTagNoPrefix;
                message.Properties["tag"] = $"{station}-{timeTagNoPrefix}";
                await queueClient.SendAsync(message);
            }
        }
    }
}
