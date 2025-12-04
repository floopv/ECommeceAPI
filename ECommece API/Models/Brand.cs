using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ECommerceAPI.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "required yastaa")]
        //[MinLength(3)]
        //[MaxLength(25)]
        public string Name { get; set; }
        //[MaxLength(150)]
        public string Description { get; set; }
        public bool Status { get; set; }
        public string Img { get; set; }
        public List<Product> Products { get; set; }
    }
}
