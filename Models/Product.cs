namespace ProductsAPI.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }=null!; // null! ifadesi null olamaz anlamına 
       
        public decimal Price { get; set; }

        public bool IsActive { get; set; }
    }
}