using Microsoft.Azure.WebJobs;

namespace DsbForsinket.DeparturesWebJob
{
    public class Program
    {
        public static void Main()
        {
            var host = new JobHost();
            host.RunAndBlock();
        }
    }
}
