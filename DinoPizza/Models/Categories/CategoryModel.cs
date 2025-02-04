using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class CategoryModel
    {
        public long CategoryId { get; set; }

        [Display(Name = "Наименование категории")]
        public string NameRus { get; set; }
    }
}
