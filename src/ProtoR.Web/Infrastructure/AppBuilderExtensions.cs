namespace ProtoR.Web.Infrastructure
{
    using System.IO;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Web.Infrastructure.Identity;

    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseIgnite(this IApplicationBuilder app)
        {
            var autofacContainer = app.ApplicationServices.GetAutofacRoot();
            var igniteFactory = autofacContainer.Resolve<IIgniteFactory>();
            igniteFactory.InitalizeIgnite();
            igniteFactory.SetAutoFacPlugin(autofacContainer);

            return app;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web App V1");
                    options.RoutePrefix = string.Empty;
                });
            }

            return app;
        }

        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    // TODO handle domain exceptions
                    if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                    {
                        await context.Response.WriteAsync("Concrete exception");
                    }

                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Internal Server Error");
                });
            });

            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            return app;
        }

        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app, IOptions<AuthenticationConfiguration> authOptions)
        {
            if (authOptions.Value.AuthenticationEnabled)
            {
                app.UseIdentityServer();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
