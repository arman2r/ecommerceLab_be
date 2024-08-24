using ecommerceLab.Models.CarritoModels;
using ecommerceLab.Models.ProductoModels;
using ecommerceLab.Models.User;
using Microsoft.EntityFrameworkCore;


namespace ecommerceLab.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<Producto> Producto { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<GetCarrito> GetCarritos { get; set; }
        public DbSet<DetalleCarrito> DetallesCarrito { get; set; }
        public DbSet<GetDetalleCarrito> GetDetallesCarrito { get; set; }

    }
}
