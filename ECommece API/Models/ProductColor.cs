using System.Drawing;

namespace ECommerceAPI.Models
{
    public class ProductColor
    {
        public int ProductId { get; set; }
        public string Color { get; set; }
        // Navigation property
        public Product Product { get; set; }
}
}
