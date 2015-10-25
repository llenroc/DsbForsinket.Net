using System;
using System.Linq;
using System.Threading.Tasks;
using DsbForsinket.Common.DsbLabs;

namespace DsbForsinket.Common.Jobs
{
    public class DeparturesWatcherJob
    {
        private const string BaseUrl = "http://traindata.dsb.dk/stationdeparture/opendataprotocol.svc";
        private const string StationId = "8600856";

        public async Task ExecuteAsync()
        {
            var service = new DSBLabsStationService(new Uri(BaseUrl));

            var delayedDeparturesQuery =
                from departure in service.Queue
                where (departure.StationUic == StationId) &&
                      (departure.Cancelled == true || departure.DepartureDelay > 0)
                select departure;

            var delayedDepartures = delayedDeparturesQuery.ToList();

            if (delayedDepartures.Any())
            {
                string message = $"Delayed trains: {delayedDepartures.Count}";
                var notificationSender = new PushNotificationSender();
                await notificationSender.SendAsync(message);
            }
        }
    }
}