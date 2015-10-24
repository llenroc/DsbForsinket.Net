using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;

namespace DsbForsinket.MobileService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            ConfigOptions options = new ConfigOptions();
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));
        }
    }
}

