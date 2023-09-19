using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(NOTEA.Startup))]
namespace NOTEA
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var serviceCollection = builder.Services;
            var configuration = builder.GetContext().Configuration;

            //builder.Services.AddSingleton<>();
        }
    }
}
