using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WallBase.Startup))]
namespace WallBase
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
