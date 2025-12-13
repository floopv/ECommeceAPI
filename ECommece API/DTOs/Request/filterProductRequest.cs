namespace ECommece_API.DTOs.Request
{
    public class filterProductRequest
    {

        public string? Name { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public bool IsHot { get; set; }
    }
}
