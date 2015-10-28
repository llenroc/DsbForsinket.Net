using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using DsbForsinket.Common;
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
            var cphTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Romance Standard Time");

            // TODO: use azure schedule instead
            if (cphTime.DayOfWeek == DayOfWeek.Saturday || cphTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return;
            }

            var minutesRounded = (cphTime.Minute / 15) * 15;
            var timeTag = $"{TimeTagPrefix}{cphTime.Hour}:{minutesRounded}";
            var timeTagNoPrefix = $"{cphTime.Hour}:{minutesRounded}";

            var stationsTags = await GetStationsTagsForTimeTag(timeTag);

            string queueName = ConfigurationManager.AppSettings["MS_QueueName"];
            string queueConnectionString = ConfigurationManager.ConnectionStrings["MS_QueueConnectionString"].ConnectionString;

            QueueClient queueClient = QueueClient.CreateFromConnectionString(queueConnectionString, queueName);

            var stations = stationsTags.Select(tag => tag.Remove(0, StationTagPrefix.Length)).ToList();
            Console.WriteLine($"Stations to watch: {stations.Count}");

            foreach (var station in stations)
            {
                var messageTag = $"{station}-{timeTagNoPrefix}";
                var message = new BrokeredMessage();
                message.Properties["station"] = station;
                message.Properties["timetag"] = timeTagNoPrefix;
                message.Properties["tag"] = messageTag;
                message.Properties["istest"] = false;
                Console.WriteLine($"Sending to queue :{messageTag}");
                await queueClient.SendAsync(message);
            }
        }

        private static async Task<HashSet<string>> GetStationsTagsForTimeTag(string timeTag)
        {
            var stationsTags = new HashSet<string>();

            foreach (var tagBucket in Tags.BucketsFromTag(timeTag))
            {
                int count = 0;
                string continuationToken = null;

                do
                {
                    var queryResult = await NotificationHubClients.Default.GetRegistrationsByTagAsync(tagBucket, continuationToken, Int32.MaxValue);
                    continuationToken = queryResult.ContinuationToken;
                    var registeredTags = queryResult
                        .SelectMany(registration => registration.Tags)
                        .Where(tag => tag.StartsWith(StationTagPrefix));

                    foreach (var tag in registeredTags)
                    {
                        stationsTags.Add(tag);
                        count++;
                    }
                } while (continuationToken != null);

                Console.WriteLine($"Registrations in bucket {tagBucket}: {count}");
            }

            return stationsTags;
        }
    }
}
