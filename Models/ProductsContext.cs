using Microsoft.EntityFrameworkCore;

namespace ProductsAPI.Models
{
    public class ProductsContext : DbContext
    {
        public ProductsContext(DbContextOptions<ProductsContext> options) : base(options)//options parametresi ile veritabanı bağlantı bilgilerini alır
        {
           
        }

         public DbSet <Product> Products { get; set; }

         // OnModelCreating metodunda Price sütunu için tip belirliyoruz
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product { ProductId = 1, ProductName = "Iphone14", Price = 5000, IsActive = true }); 
            modelBuilder.Entity<Product>().HasData(new Product { ProductId = 2, ProductName = "Iphone15", Price = 6000, IsActive = true }); 
            modelBuilder.Entity<Product>().HasData(new Product { ProductId = 3, ProductName = "Iphone16", Price = 7000, IsActive = false }); 
            modelBuilder.Entity<Product>().HasData(new Product { ProductId = 4, ProductName = "Iphone17", Price = 8000, IsActive = true }); 
            modelBuilder.Entity<Product>().HasData(new Product { ProductId = 5, ProductName = "Iphone18", Price = 8000, IsActive = true }); 

            // Price sütununun veri tipini belirleme
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)"); // Burada decimal(18, 2) kullanıyoruz
        }
    }
}