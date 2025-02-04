using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class Courier
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "ИНН должен содержать от 10 до 12 цифр.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "ИНН может содержать только цифры.")]
        [Display(Name = "ИНН")]
        public string INN { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }

        public bool IsAvailable { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
