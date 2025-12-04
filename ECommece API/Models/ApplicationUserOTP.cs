namespace ECommerceAPI.Models
{
    public class ApplicationUserOTP
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
        public DateTime ExpireAt{ get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsValid { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ApplicationUserOTP()
        {
            
        }
        public ApplicationUserOTP(string ApplicationUserId , string OTP)
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreatedAt = DateTime.UtcNow;
            this.ExpireAt = this.CreatedAt.AddMinutes(10);
            this.IsValid = true;
            this.ApplicationUserId = ApplicationUserId;
            this.OTP = OTP;
        }
    }
}
