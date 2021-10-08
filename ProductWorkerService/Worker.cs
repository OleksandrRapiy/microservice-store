using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductGrpc.Protos;
using ProductWorkerService.Factory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly ProductFactory _productFactory;
        private readonly ProductProtoService.ProductProtoServiceClient _client;

        public Worker(
            ILogger<Worker> logger, 
            IConfiguration configuration, 
            ProductFactory productFactory,
            ProductProtoService.ProductProtoServiceClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _productFactory = productFactory ?? throw new ArgumentNullException(nameof(productFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);


                Console.WriteLine("\nGet product by Id");
                Console.WriteLine(await _client.GetProductAsync(new GetProductRequest { Id = 1 }));

                Console.WriteLine("\n Create new product");
                var createdProduct = await _client.AddProductAsync(await _productFactory.GetAddProductRequestAsync());

                Console.WriteLine(await _client.GetProductAsync(new GetProductRequest { Id = createdProduct.Id }));
                await Task.Delay(_configuration.GetValue<int>("WorkerService:TaskInterval"), stoppingToken);
            }
        }
    }
}
