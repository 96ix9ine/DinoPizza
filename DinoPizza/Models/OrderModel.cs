using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DinoPizza.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Выберите клиента")]
        public Guid ClientId { get; set; }
        [ForeignKey("ClientId")]
        public ClientModel? Client { get; set; }

        [Required(ErrorMessage = "Введите имя клиента")]
        [StringLength(100, ErrorMessage = "Имя не может превышать 100 символов")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Введите адрес")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Укажите детали заказа")]
        public string? OrderDetails { get; set; }

        public DateTime OrderTime { get; set; } = DateTime.Now;

        [Required]
        public OrderStatus? Status { get; set; } = OrderStatus.Pending;

        public Guid? CourierId { get; set; }
        public Courier? Courier { get; set; }

        public DateTime? DeliveryTime { get; set; }

        [Required]
        public PaymentMethod? PaymentMethod { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public enum PaymentMethod
    {
        Card,
        Cash
    }

    public class OrderProduct
    {
        public decimal Cost { get; set; }
        public int Quantity { get; set; }
        public DinoPizza.Domains.Product Product { get; set; }
    }

}
