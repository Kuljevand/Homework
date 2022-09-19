using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Homework1.Models.AccModels
{
    public class LoginModel
    {
        [DisplayName("Корисник")]
        public string Email { get; set; }
        
        [DisplayName("Лозинка")]
        public string? Password { get; set; }
    }
}
