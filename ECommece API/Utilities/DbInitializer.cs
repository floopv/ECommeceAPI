using ECommerceAPI.DataConnection;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Utilities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ECommerceDbContext _context;
        private readonly ILogger<DbInitializer> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DbInitializer(
            ECommerceDbContext context, 
            ILogger<DbInitializer> logger, 
            RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager
            )
        {
            _context = context;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
                if (!_roleManager.Roles.Any())
                {
                    _roleManager.CreateAsync(new IdentityRole(ConstantData.Super_Admin_Role)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(ConstantData.Admin_Role)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(ConstantData.Customer_Role)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(ConstantData.Employee_Role)).GetAwaiter().GetResult();

                    _userManager.CreateAsync(new ApplicationUser()
                    {
                        UserName = "SuperAdminUser",
                        Email = "superAdmin@gmail.com",
                        FirstName = "Super",
                        LastName = "Admin",
                        EmailConfirmed = true,
                    } , "Abab1010!").GetAwaiter().GetResult();
                    var user = _userManager.FindByNameAsync("SuperAdminUser").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(user , ConstantData.Super_Admin_Role).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
