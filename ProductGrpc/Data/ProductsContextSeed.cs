using ProductGrpc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductGrpc.Data
{
    public static class ProductsContextSeed
    {
        public static void SeedAsync(this ProductsContext context)
        {
            _ = context ?? throw new NullReferenceException(nameof(context));

            if (context.Products.Any())
                return;

            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "P140",
                    Description = "New product",
                    Price = 899,
                    Status = ProductStatus.INSTOCK,
                    CreatedTime = DateTime.UtcNow
                },                
                new Product
                {
                    Id = 2,
                    Name = "AStt4",
                    Description = "New product",
                    Price = 899,
                    Status = ProductStatus.NONE,
                    CreatedTime = DateTime.UtcNow
                },                
                new Product
                {
                    Id = 3,
                    Name = "P1gregre40",
                    Description = "New produc dsf t",
                    Price = 899,
                    Status = ProductStatus.LOW,
                    CreatedTime = DateTime.UtcNow
                },
            };

            context.Products.AddRange(products);

            context.SaveChanges();
        }
    }
}
