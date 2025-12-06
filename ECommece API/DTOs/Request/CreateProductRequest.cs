namespace ECommece_API.DTOs.Request
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public IFormFile Img { get; set; }
        public List<IFormFile> SubImgs { get; set; }
        public List<string> Colors { get; set; }
    }
}
