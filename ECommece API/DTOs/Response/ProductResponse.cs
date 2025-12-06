namespace ECommece_API.DTOs.Response
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MainImg { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public List<string> Colors { get; set; } = new();
        public List<string> SubImages { get; set; } = new();
    }
}
