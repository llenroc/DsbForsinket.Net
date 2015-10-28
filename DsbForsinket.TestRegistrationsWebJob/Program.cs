using Microsoft.Azure.NotificationHubs;
using System.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DsbForsinket.TestRegistrationsWebJob
{
    public class Program
    {
        public static void Main()
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            string notificationHubName = ConfigurationManager.AppSettings["MS_NotificationHubName"];
            string notificationHubConnection = ConfigurationManager.ConnectionStrings["MS_NotificationHubConnectionString"].ConnectionString;

            var hubClient = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            var registrations = (await hubClient.GetAllRegistrationsAsync(int.MaxValue)).ToList();

            Console.WriteLine($"Registrations: {registrations.Count}");
            foreach(var reg in registrations)
            {
                Console.WriteLine("####################");
                Console.WriteLine($"Id: {reg.RegistrationId}");
                Console.WriteLine($"ExpirationTime: {reg.ExpirationTime}");
                Console.WriteLine($"Tags: {string.Join(", ", reg.Tags)}");
            }
        }
    }
}
