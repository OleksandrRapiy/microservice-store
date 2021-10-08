using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductGrpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductWorkerService.Factory
{
    public class ProductFactory
    {
        private readonly ILogger<ProductFactory> _logger;
        private readonly IConfiguration _configuration;

        public ProductFactory(ILogger<ProductFactory> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<AddProductRequest> GetAddProductRequestAsync()
        {
            var productName = _configuration.GetValue<string>("WorkerService:ProductName");
            return await Task.FromResult(new AddProductRequest
            {
                Product = new ProductModel
                {
                    Name = $"{productName} {Guid.NewGuid().ToString()}",
                    Description = Guid.NewGuid().ToString(),
                    Price = float.Parse(new Random().NextDouble().ToString()),
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow),
                    Status = ProductStatus.Low
                }
            });
        }
    }
}
