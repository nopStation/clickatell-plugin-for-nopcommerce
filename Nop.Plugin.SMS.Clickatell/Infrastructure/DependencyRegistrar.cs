using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.SMS.Clickatell.Services;

namespace Nop.Plugin.SMS.Clickatell.Infrastructure
{
    public class DependencyRegistrar : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISerderService, SerderService>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 1;
    }
}
