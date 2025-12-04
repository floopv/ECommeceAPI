using System.ComponentModel.DataAnnotations;

namespace ECommece_API.DTOs.Request
{
    public class ResetPasswordRequest
    {
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password) , Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
        public string ApplicationUserId { get; set; }
        public bool CanResetPassword { get; set; }
    }
}
