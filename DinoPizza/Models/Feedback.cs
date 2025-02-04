using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        public string? Comments { get; set; }
        public int Rating { get; set; }
    }
}
