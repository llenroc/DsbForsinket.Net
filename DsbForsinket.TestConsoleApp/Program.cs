using System;
using Microsoft.ServiceBus.Messaging;

namespace DsbForsinket.TestConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var queueConnectionString = "fill it in";
            var queueName = "fill it in";
            QueueClient queueClient = QueueClient.CreateFromConnectionString(queueConnectionString, queueName);

            var station = "8600856";
            var time = "8:00";

            while (true)
            {
                Console.WriteLine("ENTER to send");
                Console.ReadLine();
                var message = new BrokeredMessage();
                message.Properties["station"] = station;
                message.Properties["timetag"] = time;
                message.Properties["tag"] = $"{station}-{time}";
                message.Properties["istest"] = true;
                Console.WriteLine("sending...");
                queueClient.Send(message);
                Console.WriteLine("sent");
            }
        }
    }
}
