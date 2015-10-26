using System;
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

            string stationId = (string)message.Properties["station"];
            string tag = (string)message.Properties["tag"];
            var service = new DSBLabsStationService(new Uri(BaseUrl));

            log.WriteLine("Preparing the query.");

            var delayedDeparturesQuery =
                from departure in service.Queue
                where (departure.StationUic == stationId) &&
                      (departure.Cancelled == true || departure.DepartureDelay > 0)
                select departure;

            log.WriteLine("Executing the query.");
            var delayedDepartures = delayedDeparturesQuery.ToList();
            log.WriteLine("Query executed.");
            log.WriteLine($"Delayed departures {delayedDepartures.Count}.");

            if (delayedDepartures.Any())
            {
                log.WriteLine($"Sending push message to tag {tag}.");

                string pushMessage = $"Delayed trains: {delayedDepartures.Count}";
                var notificationSender = new PushNotificationSender();
                var notificationOutcome = await notificationSender.SendAsync(pushMessage, tag);

                log.WriteLine($"State: {notificationOutcome.State}");
                log.WriteLine($"Success: {notificationOutcome.Success}");
                log.WriteLine($"Failure: {notificationOutcome.Failure}");
                log.WriteLine($"NotificationId: {notificationOutcome.NotificationId}");
                log.WriteLine($"TrackingId: {notificationOutcome.TrackingId}");
            }
        }
    }
}
