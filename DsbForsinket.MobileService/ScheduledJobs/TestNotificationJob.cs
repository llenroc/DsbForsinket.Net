using System;
using System.Threading.Tasks;
using DsbForsinket.MobileService.Push;
using Microsoft.WindowsAzure.Mobile.Service;

namespace DsbForsinket.MobileService.ScheduledJobs
{
    public class TestNotificationJob : ScheduledJob
    {
        public override async Task ExecuteAsync()
        {
            var message = $"TestNotificationJob - {DateTime.UtcNow.ToShortTimeString()}";
            await new PushNotificationSender(this.Services).SendAsync(message);
        }
    }
}