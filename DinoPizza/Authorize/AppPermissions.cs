namespace DinoPizza.Authorize
{
    /// <summary>
    /// Права доступа к функционалу на сайте
    /// </summary>
    public static class AppPermissions
    {
        public static class ProductEdit
        {
            /// <summary>
            /// Право на изменение цены
            /// </summary>
            public static readonly string Price = "ProductEdit.Price";
        }
        public static class CategoryEdit
        {
            /// <summary>
            /// Право на создание новых категорий
            /// </summary>
            public static readonly string CreateCategory = "CategoryEdit.CreateCategory";
        }
    }
}
