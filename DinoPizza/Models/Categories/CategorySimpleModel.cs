using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class CategorySimpleModel
    {
        public long CategoryId { get; set; }

        [Display(Name="наименование категории")]
        public string Name { get; set; }
    }
}
