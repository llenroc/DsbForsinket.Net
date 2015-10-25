using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DsbForsinket.MobileApp.Startup))]

namespace DsbForsinket.MobileApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            new MobileAppConfiguration()
                .AddPushNotifications()
                .AddMobileAppHomeController()
                .MapApiControllers()
                .ApplyTo(config);

            app.UseWebApi(config);
        }
    }
}
