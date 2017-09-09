using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QUANLYTIEC.Startup))]
namespace QUANLYTIEC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
