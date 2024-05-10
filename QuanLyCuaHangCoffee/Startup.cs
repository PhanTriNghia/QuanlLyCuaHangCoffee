using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QuanLyCuaHangCoffee.Startup))]
namespace QuanLyCuaHangCoffee
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}
