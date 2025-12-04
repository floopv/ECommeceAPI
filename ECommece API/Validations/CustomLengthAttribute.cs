using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Validations
{
    public class CustomLengthAttribute : ValidationAttribute
    {
        private int MnLength;
        private int MxLength;
        public CustomLengthAttribute(int mnLength, int mxLength)
        {
            this.MnLength = mnLength;
            this.MxLength = mxLength;
        }
        public override bool IsValid(object? value)
        {
            if(value is string property)
            {
                return property.Length >= MnLength && property.Length <= MxLength;
            }
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be between {MnLength} and {MxLength} characters long.";
        }
    }
}
