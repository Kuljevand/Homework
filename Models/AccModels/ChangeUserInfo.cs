using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml.Linq;
 
namespace Homework1.Models.AccModels
{
    public class ChangeUserInfo
    {

        [Required()]
        public string? PhoneNumber { get; set; }
        public int ClientTypeId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? IdNo { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }
        public DateOnly DateOfEstablishment { get; set; }
        public string? NumberOfEmployees { get; set; }
        public string? Activities { get; set; }



    }
}
