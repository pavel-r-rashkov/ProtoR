namespace ProtoR.Web.Infrastructure
{
    using System.Net;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources;
    using Serilog;

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

        public static IApplicationBuilder UseCustomExceptionHandler(
            this IApplicationBuilder app,
            ILogger logger)
        {
            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    var statusCode = (int)HttpStatusCode.InternalServerError;
                    var error = "Internal Server Error";

                    if (exceptionHandlerPathFeature.Error != null)
                    {
                        var exceptionType = exceptionHandlerPathFeature.Error.GetType();

                        switch (exceptionHandlerPathFeature?.Error)
                        {
                            case InaccessibleGroupException exception:
                                statusCode = (int)HttpStatusCode.Forbidden;
                                error = exception.PublicMessage;
                                break;
                            case DomainException exception when exceptionType.IsGenericType
                                && exceptionType.GetGenericTypeDefinition() == typeof(EntityNotFoundException<>):

                                statusCode = (int)HttpStatusCode.NotFound;
                                error = exception.PublicMessage;
                                break;
                            case DomainException exception:
                                statusCode = (int)HttpStatusCode.BadRequest;
                                error = exception.PublicMessage;
                                break;
                        }

                        logger
                            .ForContext(Serilog.Core.Constants.SourceContextPropertyName, nameof(UseCustomExceptionHandler))
                            .Warning(exceptionHandlerPathFeature?.Error, "Exception handled in middleware");
                    }

                    var serializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy(),
                        },
                        NullValueHandling = NullValueHandling.Ignore,
                    };
                    var jsonResponse = JsonConvert.SerializeObject(new ErrorModel { Message = error }, serializerSettings);

                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsync(jsonResponse);
                });
            });

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

        public static IApplicationBuilder UseCustomEndpoints(this IApplicationBuilder app)
        {
            return app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHealthChecks("/hc");
            });
        }
    }
}
