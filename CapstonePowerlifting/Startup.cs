using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CapstonePowerlifting.Startup))]
namespace CapstonePowerlifting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
