namespace DinoPizza.Models
{
    public class ProductsListModel
    {
        public List<ProductSimpleModel> PageProducts { get; set; }

        public int PageCount { get; set; }

        public int PageActive { get; set; }

        public long CategoryId { get; set; }
    }
}
