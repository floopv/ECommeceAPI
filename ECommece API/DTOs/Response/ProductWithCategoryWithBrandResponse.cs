namespace ECommece_API.DTOs.Response
{
    public class ProductWithCategoryWithBrandResponse
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public Product? Product { get; set; }
    }
}
