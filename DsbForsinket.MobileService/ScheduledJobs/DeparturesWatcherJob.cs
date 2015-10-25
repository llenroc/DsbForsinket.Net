using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using DsbForsinket.MobileService.DsbLabs;
using DsbForsinket.MobileService.Push;
using Microsoft.WindowsAzure.Mobile.Service;

namespace DsbForsinket.MobileService.ScheduledJobs
{
    public class DeparturesWatcherJob : ScheduledJob
    {
        private const string BaseUrl = "http://traindata.dsb.dk/stationdeparture/opendataprotocol.svc";
        private const string StationId = "8600856";

        public override async Task ExecuteAsync()
        {
            try
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
                    var notificationSender = new PushNotificationSender(this.Services);
                    await notificationSender.SendAsync(message, TimeSpan.FromMinutes(3));
                }

                Services.Log.Info("Completed execution.");
            }
            catch (Exception ex)
            {
                Services.Log.Error(ex);
            }
        }
    }
}