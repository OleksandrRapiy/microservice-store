using AutoMapper;
using DiscountGrpc.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingCartGrpc.Data;
using ShoppingCartGrpc.Models;
using ShoppingCartGrpc.Protos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingCartGrpc.Services
{
    [Authorize]
    public class ShoppingCartService : ShoppingCartProtoService.ShoppingCartProtoServiceBase
    {
        private readonly ShoppingCartContext _context;
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountService;
        private readonly ILogger<ShoppingCartService> _logger;
        private readonly IMapper _mapper;

        public ShoppingCartService(ShoppingCartContext context,
            ILogger<ShoppingCartService> logger,
            DiscountProtoService.DiscountProtoServiceClient discountService,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<ShoppingCartModel> GetShoppingCart(GetShoppingCartRequest request, ServerCallContext context)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserName == request.UserName);

            _ = shoppingCart ?? throw new RpcException(new Status(StatusCode.NotFound, $"Shopping cart for user - {request.UserName} not found"));

            return _mapper.Map<ShoppingCartModel>(shoppingCart);
        }

        public override async Task<ShoppingCartModel> CreateShoppingCart(ShoppingCartModel request, ServerCallContext context)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var shoppingCart = _mapper.Map<ShoppingCart>(request);

            if (await _context.ShoppingCarts.AnyAsync(x => x.UserName == request.UserName))
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Shopping cart for user - {request.UserName} not found"));
            }

            _context.ShoppingCarts.Add(shoppingCart);

            await _context.SaveChangesAsync();

            return request;
        }

        public override async Task<RemoveItemFromCartResponse> RemoveItemFromShoppingCart(RemoveItemFromShoppingCartRequest request, ServerCallContext context)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserName == request.UserName);

            _ = shoppingCart ?? throw new RpcException(new Status(StatusCode.NotFound, $"Shopping cart for user - {request.UserName} not found"));

            var itemToRemove = shoppingCart.Items.FirstOrDefault(x => x.ProductId == request.RemovedCartItems.ProductId);

            _ = itemToRemove ?? throw new RpcException(new Status(StatusCode.NotFound, $"Cart item with product id - {request.RemovedCartItems.ProductId} not found"));

            _context.ShoppingCartItems.Remove(itemToRemove);

            await _context.SaveChangesAsync();

            return new RemoveItemFromCartResponse { Success = true };
        }

        public override async Task<AddItemIntoShoppingCartResponse> AddItemIntoShoppingCart(IAsyncStreamReader<AddItemIntoShoppingCartRequest> requestStream, ServerCallContext context)
        {
            _ = requestStream ?? throw new ArgumentNullException(nameof(requestStream));
            _ = context ?? throw new ArgumentNullException(nameof(context));

            while (await requestStream.MoveNext())
            {
                var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserName == requestStream.Current.UserName);

                _ = shoppingCart ?? throw new RpcException(new Status(StatusCode.NotFound, $"Shopping cart for user - {requestStream.Current.UserName} not found"));

                var newCartItem = _mapper.Map<ShoppingCartItem>(requestStream.Current.NewCartItem);
                var cartItem = shoppingCart.Items.FirstOrDefault(x => x.ProductId == requestStream.Current.NewCartItem.ProductId);

                if (cartItem != null)
                {
                    cartItem.Quantity++;
                }
                else
                {
                    // Call to discount gRPC service

                    var discount = await _discountService.GetDiscountAsync(new GetDiscountRequest { Code = requestStream.Current.Discount });
                    newCartItem.Price -= discount.Amount;

                    shoppingCart.Items.Add(newCartItem);
                }
            }

            var inserCount = await _context.SaveChangesAsync();

            return new AddItemIntoShoppingCartResponse
            {
                Success = inserCount > 0,
                InsertCount = inserCount,
            };
        }
    }
}
