namespace ECommece_API.DTOs.Response
{
    public class ProductWithRelatedProductsResponse
    {

        public Product Product { get; set; }
        public IEnumerable<Product> RelatedProducts { get; set; } = new List<Product>();
    }
}
