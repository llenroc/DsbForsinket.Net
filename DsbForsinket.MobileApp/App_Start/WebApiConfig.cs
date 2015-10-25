using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace DsbForsinket.MobileApp
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            HttpConfiguration config = new HttpConfiguration();
            new MobileAppConfiguration()
                .AddPushNotifications()
                .AddMobileAppHomeController()
                .MapApiControllers()
                .ApplyTo(config);
        }
    }
}

