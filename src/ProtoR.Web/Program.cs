namespace ProtoR.Web
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using ProtoR.Web.Infrastructure;
    using Serilog;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .CustomConfiguration()
                .CreateLogger();

            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder
                        .UseStartup<Startup>()
                        .ConfigureKestrel((webHostContext, serverOptions) =>
                        {
                            var tlsConfiguration = new TlsConfiguration();
                            webHostContext.Configuration
                                .GetSection(nameof(TlsConfiguration))
                                .Bind(tlsConfiguration);

                            if (!string.IsNullOrEmpty(tlsConfiguration.CertificateLocation))
                            {
                                serverOptions.ConfigureHttpsDefaults(httpsOptions =>
                                {
                                    httpsOptions.ServerCertificate = new X509Certificate2(
                                        tlsConfiguration.CertificateLocation,
                                        tlsConfiguration.Password);
                                });
                            }
                        });
                })
                .UseSerilog();
        }
    }
}
