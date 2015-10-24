using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using System.Collections.Generic;
using System;

namespace DsbForsinket.MobileService.ScheduledJobs
{
    public class TestNotificationJob : ScheduledJob
    {
        public override async Task ExecuteAsync()
        {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                ["message"] = "TestNotificationJob " + DateTime.UtcNow.ToShortTimeString()
            };

            GooglePushMessage message = new GooglePushMessage(data, TimeSpan.FromMinutes(1));

            try
            {
                var result = await Services.Push.SendAsync(message);
                Services.Log.Info(result.State.ToString());
            }
            catch (Exception ex)
            {
                Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
            }
        }
    }
}