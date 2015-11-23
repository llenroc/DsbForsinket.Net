using System;
using System.Collections.Generic;
using DsbForsinket.Common;

namespace DsbForsinket.WebJob
{
    public class Program
    {
        public static void Main()
        {
            var tag = "8600856-10:45";

            Dictionary<string, string> messageData = new Dictionary<string, string>
            {
                ["delayedCount"] = "4",
                ["departureName0"] = "Test Train",
                ["departureTime0"] = "11:23",
                ["departureDelay0"] = "0",
                ["departureName1"] = "Marcin is testing",
                ["departureTime1"] = "00:03",
                ["departureDelay1"] = "5",
                ["departureName2"] = "Train 3",
                ["departureTime2"] = "",
                ["departureDelay2"] = "10",
                ["departureName3"] = "Train 4",
                ["departureTime3"] = "11:23",
                ["departureDelay3"] = "61"
            };

            var notificationOutcome = new PushNotificationSender(Console.WriteLine).SendAsync(messageData, tag).Result;
            Console.WriteLine($"State: {notificationOutcome.State}");
            Console.WriteLine($"Success: {notificationOutcome.Success}");
            Console.WriteLine($"Failure: {notificationOutcome.Failure}");
            Console.WriteLine($"NotificationId: {notificationOutcome.NotificationId}");
            Console.WriteLine($"TrackingId: {notificationOutcome.TrackingId}");
        }
    }
}
