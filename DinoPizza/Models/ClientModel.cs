using DinoPizza.Authorize;
using System.ComponentModel.DataAnnotations;

namespace DinoPizza.Models
{
    public class ClientModel
    {
        [Key]
        public Guid ClientId { get; set; }
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public virtual AppUser User { get; set; }
    }
}
