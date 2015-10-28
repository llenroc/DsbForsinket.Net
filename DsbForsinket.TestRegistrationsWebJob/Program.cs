using System;
using System.Linq;
using System.Threading.Tasks;
using DsbForsinket.Common;

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
            var registrations = (await NotificationHubClients.Default.GetAllRegistrationsAsync(int.MaxValue)).ToList();

            Console.WriteLine($"Registrations: {registrations.Count}");
            foreach (var reg in registrations)
            {
                Console.WriteLine("####################");
                Console.WriteLine($"Id: {reg.RegistrationId}");
                Console.WriteLine($"ExpirationTime: {reg.ExpirationTime}");
                Console.WriteLine($"Tags: {string.Join(", ", reg.Tags)}");
            }
        }
    }
}
