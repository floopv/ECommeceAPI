namespace ECommerceAPI.Models
{
    public class ProductSubImage

    {
        public int ProductId { get; set; }
        public string Img { get; set; }
        // Navigation property
        public Product Product { get; set; }
}
}
