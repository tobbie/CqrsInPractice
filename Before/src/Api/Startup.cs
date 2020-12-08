using Api.Utils;
using Logic.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Logic.Students;
using System.Collections.Generic;
using Logic.Dtos;
using Logic.Decorators;

namespace Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var config = new Config(3); // in prod, get from appsettings.json.
            services.AddSingleton(config);
            var commandsConnectionString = new CommandsConnectionString(Configuration["CommandsConnectionString"]);
            var queriesConnectionString = new QueriesConnectionString(Configuration["QueriesConnectionString"]);
            services.AddSingleton(commandsConnectionString);
            services.AddSingleton(queriesConnectionString);
            services.AddSingleton<SessionFactory>();
            
            services.AddSingleton<Messages>();
            services.AddHandlers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandler>();
            app.UseMvc();
        }
    }
}
