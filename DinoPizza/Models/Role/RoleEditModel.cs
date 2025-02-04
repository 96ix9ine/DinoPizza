using Mapster;
using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class RoleEditModel
    {
        [Display(Name = "Идентификатор роли")]
        public string Id { get; set; }

        [Display(Name = "Наименование роли")]
        public string Name { get; set; }

        [AdaptIgnore]
        public List<CheckBoxItemStringId> PermissionsList { get; set; }

        [AdaptIgnore]
        public List<CheckBoxItemStringId> UsersList { get; set; }
    }
}
