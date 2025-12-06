using ECommerceAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace ECommece_API.DTOs.Request
{
    public class UpdateBrandRequest
    {
        [Required(ErrorMessage = "required yastaa")]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }
        public bool Status { get; set; }
        public string Img { get; set; } = "default.png";

        [AllowedExtensions(new[] { ".jpg", ".png", ".jpeg", ".gif" })]
        public IFormFile? FormImg { get; set; }
    }
}
