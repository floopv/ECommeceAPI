namespace ECommece_API.DTOs.Response
{
    public class ProductWithOptionsResponse
    {

        public ProductResponse Product { get; set; }
        public List<CategoryResponse> Categories { get; set; } = new();
        public List<BrandResponse> Brands { get; set; } = new();
    }
}
