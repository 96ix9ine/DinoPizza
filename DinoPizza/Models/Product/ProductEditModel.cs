using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class ProductEditModel
    {
        [Display(Name="Картинка для товара")]
        [AdaptIgnore]
        public IFormFile? FileImage { get; set; }

        [AdaptIgnore]
        public long SelectedCategoryId { get; set; } // Выбранная категория

        [AdaptIgnore]
        [ValidateNever]
        public List<SelectListItem> CategoriesList { get; set; } // Список категорий

        [AdaptIgnore]
        [HiddenInput(DisplayValue = false)]
        [ValidateNever]
        public string? ReturnUrl { get; set; } // URL возврата

        [Display(Name = "Идентификатор товара")]
        public long ProductId { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Цена")]
        [DataType(DataType.Currency)]
        public int Price { get; set; }

        [Display(Name = "Артикул")]
        public string Article { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Display(Name = "Количество на складе")]
        public int QuantityStock { get; set; }

        [Display(Name = "Url картинки")]
        public string? ImageUrl { get; set; }

        public string ImageSrc
        {
            get
            {
                if (String.IsNullOrEmpty(ImageUrl))
                {
                    return $"{ModelConstants.Folder}\\{ModelConstants.NoImage}";
                }
                else
                {
                    return $"{ModelConstants.Folder}\\{ImageUrl}";
                }
            }
        }
    }
}
