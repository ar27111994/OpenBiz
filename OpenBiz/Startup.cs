using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OpenBiz.Startup))]
namespace OpenBiz
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
