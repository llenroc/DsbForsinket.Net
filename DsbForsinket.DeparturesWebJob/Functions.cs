using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DsbForsinket.Common;
using DsbForsinket.Common.DsbLabs;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;

namespace DsbForsinket.DeparturesWebJob
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

            var delayedDeparturesQuery =
                from departure in service.Queue
                where (departure.StationUic == stationId) &&
                      (departure.Cancelled == true || departure.DepartureDelay > 0)
                select departure;

            var delayedDepartures = delayedDeparturesQuery.ToList();

            if (delayedDepartures.Any() || true) // TODO: temporary
            {
                string pushMessage = $"Delayed trains: {delayedDepartures.Count}";
                var notificationSender = new PushNotificationSender();
                await notificationSender.SendAsync(pushMessage, tag);
            }
        }
    }
}
