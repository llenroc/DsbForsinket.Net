using System.Threading.Tasks;
using DsbForsinket.Common.Jobs;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Notifications;

namespace DsbForsinket.WebJob
{
    public class Program
    {
        public static void Main()
        {
            new TestNotificationJob()
                    .ExecuteAsync()
                    .Wait();
        }
    }
}
