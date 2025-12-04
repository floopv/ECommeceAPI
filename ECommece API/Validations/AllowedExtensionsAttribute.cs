using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Validations
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private string[] allowedExtensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            this.allowedExtensions = extensions;
        }
        public override bool IsValid(object? value)
        {
            if (value is IFormFile FormImg)
            {
                var fileExtension = Path.GetExtension(FormImg.FileName).ToLower();
                return allowedExtensions.Contains(fileExtension);
            }
            if (value is null)
                return true;
            return false;
        }
    }
}
