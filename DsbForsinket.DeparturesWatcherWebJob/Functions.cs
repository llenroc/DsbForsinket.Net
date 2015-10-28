using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using DsbForsinket.Common;
using DsbForsinket.Common.DsbLabs;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;

namespace DsbForsinket.DeparturesWatcherWebJob
{
    public class Functions
    {
        private const string BaseUrl = "http://traindata.dsb.dk/stationdeparture/opendataprotocol.svc";

        public static async Task ProcessQueueMessage([ServiceBusTrigger("dsbforsinketqueue")] BrokeredMessage message, TextWriter log)
        {
            log.WriteLine($"Got message: {message}");
            var isDebugMode = Convert.ToBoolean(ConfigurationManager.AppSettings["APP_DEBUG_MODE"]);
            log.WriteLine($"DEBUG MODE: {isDebugMode}");

            string stationId = (string)message.Properties["station"];
            string tag = (string)message.Properties["tag"];
            bool isTestSend = (bool)message.Properties["istest"];
            var service = new DSBLabsStationService(new Uri(BaseUrl));

            log.WriteLine($"stationId: {stationId} tag: {tag} isTestSend: {isTestSend}");
            log.WriteLine("Preparing the query.");

            var delayedDeparturesQuery =
                from departure in service.Queue
                where (departure.StationUic == stationId) &&
                      (departure.Cancelled == true || departure.DepartureDelay > 0 || isDebugMode)
                select departure;

            log.WriteLine("Executing the query.");
            var delayedDepartures = delayedDeparturesQuery.ToList();
            log.WriteLine("Query executed.");
            log.WriteLine($"Delayed departures: {delayedDepartures.Count}.");

            if (delayedDepartures.Any() || isTestSend || isDebugMode)
            {
                log.WriteLine($"Sending push message to tag: {tag}.");

                delayedDepartures = delayedDepartures.OrderBy(d => d.ScheduledDeparture).ToList();

                Dictionary<string, string> messageData = new Dictionary<string, string>
                {
                    ["delayedCount"] = delayedDepartures.Count.ToString(CultureInfo.InvariantCulture)
                };

                foreach (var departure in delayedDepartures.Take(5).Select((data, index) => new { index, data }))
                {
                    var destinationName = string.IsNullOrWhiteSpace(departure.data.Line)
                                                ?  departure.data.DestinationName
                                                : $"{departure.data.Line} <i>{departure.data.DestinationName}</i>";
                    messageData[$"departureName{departure.index}"] = destinationName;
                    long delayInMinutes = (departure.data.DepartureDelay / 60) ?? 0;
                    messageData[$"departureDelay{departure.index}"] = Convert.ToString(delayInMinutes);
                }

                var notificationSender = new PushNotificationSender(log.WriteLine);

                var notificationOutcome = await notificationSender.SendAsync(messageData, tag);

                log.WriteLine($"State: {notificationOutcome.State}");
                log.WriteLine($"Success: {notificationOutcome.Success}");
                log.WriteLine($"Failure: {notificationOutcome.Failure}");
                log.WriteLine($"NotificationId: {notificationOutcome.NotificationId}");
                log.WriteLine($"TrackingId: {notificationOutcome.TrackingId}");
            }
        }
    }
}
