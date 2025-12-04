using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerceAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MainImg { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public bool Status { get; set; }
        public decimal Discount { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        // Navigation properties
        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public ICollection<ProductSubImage> ProductSubImages { get; set; }
        public ICollection<ProductColor> ProductColors { get; set; }
    }
}
