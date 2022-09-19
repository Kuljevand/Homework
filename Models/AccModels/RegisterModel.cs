using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Homework1.Models.AccModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="Задолжително")]
        [Display(Name = "E-маил")]
        public string? Email { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public int ClientTypeId { get; set; }
        public string Name { get; set; } = null!;
        [Required]
        public string Address { get; set; } = null!;
        public string IdNo { get; set; } = null!;
        public int CityId { get; set; }
        public int? CountryId { get; set; }
    }
}
