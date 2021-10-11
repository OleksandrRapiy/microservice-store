using ShoppingCartGrpc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCartGrpc.Data
{
    public static class ShoppingCartContextSeed
    {
        public static void SeedAsync(this ShoppingCartContext context)
        {
            _ = context ?? throw new NullReferenceException(nameof(context));

            if (context.ShoppingCarts.Any())
                return;

            var products = new List<ShoppingCart>
            {
                new ShoppingCart
                {
                    UserName = "Jhon",
                    Items = new List<ShoppingCartItem>()
                    {
                        new ShoppingCartItem
                        {
                            Quantity = 2,
                            Color = "Black",
                            Price = 132,
                            ProductId = 1,
                            ProductName = "M1 CPU"
                        },
                        new ShoppingCartItem
                        {
                            Quantity = 10,
                            Color = "Black",
                            Price = 132,
                            ProductId = 1,
                            ProductName = "M2 CPU"
                        }
                    }
                },
            };

            context.ShoppingCarts.AddRange(products);

            context.SaveChanges();
        }
    }
}
