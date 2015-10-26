using System.IO;
using Microsoft.Azure.WebJobs;

namespace DsbForsinket.DeparturesWebJob
{
    public class Functions
    {
        public static void ProcessQueueMessage([QueueTrigger("dsbforsinketqueue")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }
    }
}
