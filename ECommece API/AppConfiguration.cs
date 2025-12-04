using ECommerceAPI.DataConnection;
using ECommerceAPI.Models;
using ECommerceAPI.Repos;
using ECommerceAPI.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI
{
    public static class AppConfiguration
    {
        public static void Config(this IServiceCollection Services , string connectionString)
        {
            Services.AddTransient<IEmailSender, EmailSender>();
            Services.AddScoped<IRepository<Category>, Repository<Category>>();
            Services.AddScoped<IRepository<Brand>, Repository<Brand>>();
            Services.AddScoped<IRepository<Product>, Repository<Product>>();
            Services.AddScoped<IRepository<ProductSubImage>, Repository<ProductSubImage>>();
            Services.AddScoped<IRepository<ProductColor>, Repository<ProductColor>>();  
            Services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();  
            Services.AddScoped<IRepository<Cart> , Repository<Cart>>();  
            Services.AddScoped<IRepository<Promotion> , Repository<Promotion>>();  
            Services.AddScoped<IDbInitializer, DbInitializer>();  

            Services.AddDbContext<ECommerceDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });


            Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ECommerceDbContext>();
        }
    }
}
