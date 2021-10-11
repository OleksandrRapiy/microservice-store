using Microsoft.EntityFrameworkCore;
using ShoppingCartGrpc.Models;

namespace ShoppingCartGrpc.Data
{
    public class ShoppingCartContext: DbContext
    {
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public ShoppingCartContext(DbContextOptions<ShoppingCartContext> options) : base(options)
        { }
    }
}
