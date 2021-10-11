using DiscountGrpc.Models;
using System.Collections.Generic;

namespace DiscountGrpc.Data
{
    public class DiscountContext
    {
        public static List<Discount> Discounts { get; } = new List<Discount>()
        {
            new Discount() { Id = 1, Code = "Code-1", Amount = 100 },
            new Discount() { Id = 2, Code = "Code-2", Amount = 200 },
            new Discount() { Id = 3, Code = "Code-3", Amount = 300 }
        };
    }
}
