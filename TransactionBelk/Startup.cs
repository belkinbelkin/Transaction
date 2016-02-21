using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TransactionBelk.Startup))]
namespace TransactionBelk
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
