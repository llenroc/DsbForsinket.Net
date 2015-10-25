using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;

namespace DsbForsinket.MobileService.Push
{
    public class PushNotificationSender
    {
        public ApiServices Services { get; }

        public PushNotificationSender(ApiServices services)
        {
            this.Services = services;
        }

        public async Task SendAsync(string message, TimeSpan timeToLive)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    ["message"] = message
                };

                GooglePushMessage pushMessage = new GooglePushMessage(data, timeToLive);
                var result = await this.Services.Push.SendAsync(pushMessage);
                this.Services.Log.Info(result.State.ToString());
            }
            catch (Exception ex)
            {
                this.Services.Log.Error(ex);
            }
        }
    }
}