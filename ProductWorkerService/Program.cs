using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductGrpc.Protos;
using ProductWorkerService.Factory;
using System;

namespace ProductWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddGrpcClient<ProductProtoService.ProductProtoServiceClient>(
                        (x, opt) =>
                        {
                            var config = hostContext.Configuration;
                            opt.Address = new Uri(config.GetValue<string>("WorkerService:ServiceUrl"));
                        });

                    services.AddTransient<ProductFactory>();

                    services.AddHostedService<Worker>();
                });
    }
}
