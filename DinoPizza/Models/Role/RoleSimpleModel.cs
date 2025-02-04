using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class RoleSimpleModel
    {
        [Display(Name = "Идентификатор роли")]
        public string Id { get; set; }

        [Display(Name = "Наименование роли")]
        public string Name { get; set; }
    }
}
