using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class ProductSimpleModel
    {
        public long Id { get; set; }

        [Display(Name = "Наименование товара")]
        public string Name { get; set; }

        [Display(Name = "Цена")]
        [DataType(DataType.Currency)]
        public int Price { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public string ImageSrc { get; set; }
    }
}
