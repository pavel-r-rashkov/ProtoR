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
    using Newtonsoft.Json.Linq;
    using ProtoR.Domain.Exceptions;
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

        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
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
                            case InaccessibleCategoryException exception:
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
                    }

                    // TODO Log exception
                    var jsonResponse = JObject.FromObject(new { error });
                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsync(jsonResponse.ToString());
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
    }
}
