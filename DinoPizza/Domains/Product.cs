using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DinoPizza.Abstract;

namespace DinoPizza.Domains
{
    [Table("Product")]
    public class Product : MyEntity<long>
    {
        public override long GetId() => ProductId;

        [Key]
        public long ProductId { get; set; }

        public string? Description { get; set; }

        public string? Name { get; set; }

        public int Price { get; set; }

        public string? Article { get; set; }

        public string? ImageUrl { get; set; }

        public long? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
