using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ProductGrpc.Data;
using ProductGrpc.Protos;
using System;
using System.Threading.Tasks;

namespace ProductGrpc.Services
{
    public class ProductService : ProductProtoService.ProductProtoServiceBase
    {
        private readonly ProductsContext _context;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;


        public ProductService(ProductsContext context, ILogger<ProductService> logger, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<ProductModel> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var product = await _context.Products.FindAsync(request.Id);

            if (product == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Not found object with id - {request.Id}"));
            }

            return _mapper.Map<ProductModel>(product);
        }

        public override async Task<ProductModel> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var product = _mapper.Map<Models.Product>(request.Product);

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return _mapper.Map<ProductModel>(product);
        }
    }
}
