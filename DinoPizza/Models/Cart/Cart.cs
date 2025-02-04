using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using DinoPizza.BusinessLogic;
using System.ComponentModel.DataAnnotations.Schema;


namespace DinoPizza.Models
{
    public class Cart
    {
        [Key]  // Убедитесь, что у модели есть ключ, если его нет
        public Guid Id { get; set; } = Guid.NewGuid();  // Уникальный идентификатор корзины

        public Guid ClientId { get; set; }  // Идентификатор клиента

        public string? OrderDetails { get; set; }

        // Связь с записями в корзине
        public List<CartRecord> Records { get; set; } = new List<CartRecord>();

        public int TotalQuantity => Records.Sum(r => r.Quantity);

        public int TotalCost => Records.Sum(r => r.Cost);

        public void Clear() => Records.Clear();

        public void AddProduct(ProductSimpleModel model)
        {
            var record = Records.FirstOrDefault(p => p.Product.Id == model.Id);
            if (record == null)
            {
                record = new CartRecord { Product = model, Quantity = 1 };
                Records.Add(record);
            }
            else
            {
                record.Quantity++;
            }
        }

        public void RemoveProduct(ProductSimpleModel model)
        {
            var record = Records.FirstOrDefault(p => p.Product.Id == model.Id);
            if (record != null)
            {
                record.Quantity--;
                if (record.Quantity == 0)
                {
                    Records.Remove(record);
                }
            }
        }

        public void RemoveRecord(ProductSimpleModel model)
        {
            var record = Records.FirstOrDefault(p => p.Product.Id == model.Id);
            if (record != null)
            {
                Records.Remove(record);
            }
        }
    }

    [NotMapped]
    public class CartRecord
    {
        public int Cost => Quantity * Product.Price;

        public int Quantity { get; set; }

        public ProductSimpleModel Product { get; set; }
    }
}
