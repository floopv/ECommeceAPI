using static ECommece_API.Utilities.Enums;

namespace ECommece_API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public TransactionType TransactionType { get; set; } = TransactionType.Card;
        public string? SessionId { get; set; }
        public string? TransactionId { get; set; }
    }
}
