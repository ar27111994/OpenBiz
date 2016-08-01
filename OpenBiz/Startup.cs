using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SCMS.Startup))]
namespace SCMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
