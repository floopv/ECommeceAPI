//using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Mapster;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ECommerce.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfig(this IServiceCollection services)
        {
            TypeAdapterConfig<ApplicationUser, ApplicationUserVM>.NewConfig().
                Map(dest=> dest.FullName , src=> $"{src.FirstName} {src.LastName}");
        }
    }
}
