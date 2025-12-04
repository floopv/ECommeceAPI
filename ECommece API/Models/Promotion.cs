namespace ECommerceAPI.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public decimal Discount { get; set; }
        public string Code { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsValid { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int MaxUsage { get; set; }
    }
}
