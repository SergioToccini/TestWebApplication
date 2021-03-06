using System.Reflection;
using System.Text.Json.Serialization;
using CorrelationId;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestCleanApplication.Domain.Base;
using RestCleanApplication.Extensions;

namespace RestCleanApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCorrelationId();
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            services.AddSwaggerDocumentation();

            services
                .AddControllers()
                .AddFluentValidation(x => x
                    .RegisterValidatorsFromAssemblyContaining(typeof(IViewModel))
                    .RunDefaultMvcValidationAfterFluentValidationExecutes = true)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseSwaggerDocumentation();
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "X-Correlation-Id",
                UseGuidForCorrelationId = true,
                UpdateTraceIdentifier = false,
                IncludeInResponse = true
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
