using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Homework1.Models.AccModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="Задолжително поле")]
        [Display(Name = "E-маил")]
        public string? Email { get; set; }
        [Required()]
        public string? PhoneNumber { get; set; }
        [Required()]
        public int ClientTypeId { get; set; }
        [Required()]
        public string Name { get; set; }
        [Required()]
        public string Address { get; set; }
        [Required()]
        public string IdNo { get; set; }
        [Required()]
        public int CityId { get; set; }
        [Required()]
        public int? CountryId { get; set; }
        [Required()]
        public string? Role { get; set; }
        [Required()]
        public DateTime DateOfEstablishment { get; set; } = DateTime.Today; 
        [Required()]
        public string? NumberOfEmployees { get; set; }
        [Required()]
        public string? Activities { get; set; }


    }
}
