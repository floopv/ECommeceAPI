using ECommece_API.DTOs.Response;
using Mapster;
using Microsoft.Extensions.Logging.Abstractions;

namespace ECommece_API.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfig(this IServiceCollection services)
        {
            TypeAdapterConfig<ApplicationUser, ApplicationUserResponse>.NewConfig().
                Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");
            TypeAdapterConfig<Product, ProductResponse>.NewConfig().
                Map(dest => dest.Colors ,src => src.ProductColors != null ? src.ProductColors.Select(pc=>pc.Color) : new List<string>()).
                Map(dest => dest.SubImages ,src => src.ProductSubImages != null ? src.ProductSubImages.Select(psi=>psi.Img) : new List<string>());

        }
    }
}
