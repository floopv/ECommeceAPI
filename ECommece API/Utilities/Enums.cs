namespace ECommece_API.Utilities
{
    public class Enums
    {
        public enum OrderStatus
        {
            Pending,
            InProgress,
            Shipped,
            Completed,
            Canceled
        }
        public enum TransactionType
        {
            Card , 
            Cash
        }

    }
}
