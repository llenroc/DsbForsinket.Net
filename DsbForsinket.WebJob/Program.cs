using System;
using DsbForsinket.Common;

namespace DsbForsinket.WebJob
{
    public class Program
    {
        public static void Main()
        {
            var message = $"TestNotificationJob - {DateTime.UtcNow.ToShortTimeString()}";
            var notificationOutcome = new PushNotificationSender().SendAsync(message).Result;
            Console.WriteLine($"State: {notificationOutcome.State}");
            Console.WriteLine($"Success: {notificationOutcome.Success}");
            Console.WriteLine($"Failure: {notificationOutcome.Failure}");
            Console.WriteLine($"NotificationId: {notificationOutcome.NotificationId}");
            Console.WriteLine($"TrackingId: {notificationOutcome.TrackingId}");
        }
    }
}
