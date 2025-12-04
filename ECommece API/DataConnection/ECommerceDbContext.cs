using ECommerceAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
//using ECommerceAPI.DTOs;

namespace ECommerceAPI.DataConnection
{
    public class ECommerceDbContext : IdentityDbContext<ApplicationUser>
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSubImage> ProductSubImages { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        //override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=FLOOPV\\SQLEXPRESS;initial catalog = ECommerce ;Integrated Security=True;" +
        //        "Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;" +
        //        "Multi Subnet Failover=False");
        //}
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductColor>().
                HasKey(pc => new
                {
                    pc.ProductId,
                    pc.Color
                });

            modelBuilder.Entity<ProductSubImage>().
                        HasKey(ps => new
                             {
                               ps.ProductId,
                                ps.Img
                                });
            modelBuilder.Entity<Brand>().
                Property(b=>b.Img).HasDefaultValue("defaultImg.png");
            modelBuilder.Entity<Cart>().HasKey(c => new
            {
                c.ApplicationUserId,
                c.ProductId
            });
        }
    }
}
