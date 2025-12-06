namespace ECommece_API.DTOs.Request
{
    public class UpdateProductRequest
    {
        public string Name { get; set; }              
        public decimal Price { get; set; }           
        public string Description { get; set; }     
        public IFormFile MainImg { get; set; }      
        public List<IFormFile> SubImages { get; set; } = new List<IFormFile>(); 
        public List<string> Colors { get; set; } = new List<string>();
    }
}
