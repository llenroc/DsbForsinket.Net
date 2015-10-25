using System;
using System.Threading.Tasks;

namespace DsbForsinket.Common.Jobs
{
    public class TestNotificationJob
    {
        public async Task ExecuteAsync()
        {
            var message = $"TestNotificationJob - {DateTime.UtcNow.ToShortTimeString()}";
            await new PushNotificationSender().SendAsync(message);
        }
    }
}