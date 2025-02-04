using Mapster;
using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class UserSimpleModel
    {
        [Display(Name ="Идентификатор пользователя")]
        public string UserId { get; set; }

        [Display(Name = "Логин пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Роли")]
        [AdaptIgnore]
        public List<string> RolesList { get; set; }
    }
}
