    namespace ProductsAPI.DTO
    {
        public class ProductDTO
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }=null!; // null! ifadesi null olamaz anlamına 
            public decimal Price { get; set; }
        }
    }