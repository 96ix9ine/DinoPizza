using Mapster;
using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class UserEditModel
    {
        [Display(Name = "Идентификатор пользователя")]
        public string UserId { get; set; }

        [Display(Name = "Логин пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Электронная почта")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Номер телефона")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "Имя пользователя")]
        public string? FirstName { get; set; }

        [Display(Name = "Почтовый адресс")]
        public string? Address { get; set; }

        [AdaptIgnore]
        public List<CheckBoxItemStringId> RolesList { get; set; }
    }
}
