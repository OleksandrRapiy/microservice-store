using DiscountGrpc.Data;
using DiscountGrpc.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscountGrpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(ILogger<DiscountService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<DiscountModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var discount = DiscountContext.Discounts.FirstOrDefault(x => x.Code == request.Code);

            _ = discount ?? throw new RpcException(new Status(StatusCode.NotFound, $"Discount by code - {request.Code} not found"));

            return new DiscountModel
            {
                Id = discount.Id,
                Code = discount.Code,
                Amount = discount.Amount
            };
        }
    }
}
