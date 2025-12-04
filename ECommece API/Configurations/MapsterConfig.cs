using ECommece_API.DTOs.Response;
using Mapster;

namespace ECommece_API.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfig(this IServiceCollection services)
        {
            TypeAdapterConfig<ApplicationUser, ApplicationUserResponse>.NewConfig().
                Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");
        }
    }
}
