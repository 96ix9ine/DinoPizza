using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class ProductDetailsModel
    {
        [AdaptIgnore]
        [HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }

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
