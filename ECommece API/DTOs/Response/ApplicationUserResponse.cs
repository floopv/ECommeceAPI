using System.ComponentModel.DataAnnotations;

namespace ECommece_API.DTOs.Response
{
    public class ApplicationUserResponse
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]

        public string? NewPassword { get; set; }
    }
}
