using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DinoPizza.Abstract;

namespace DinoPizza.Domains
{
    [Table("Category")]
    public class Category : MyEntity<long>
    { 

        public override long GetId() => CategoryId;

        [Key]
        public long CategoryId { get; set; }

        public string NameRus { get; set; }

        public bool IsActive { get; set; }

    }
}
