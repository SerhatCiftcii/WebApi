using Microsoft.AspNetCore.Mvc;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
 {
    //localhost:5001/api/products
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static List<Product>? _products; 
        public ProductsController()
        {
            _products =
            [
                new Product{ProductId=1,ProductName="Iphone14",Price=5000,IsActive=true},
                new Product{ProductId=2,ProductName="Iphone15",Price=6000,IsActive=true},
                new Product{ProductId=3,ProductName="Iphone16",Price=7000,IsActive=true},
                new Product{ProductId=4,ProductName="Iphone17",Price=8000,IsActive=true},
            ];//refrrans tip olduğu için new ile bellekte yer açıyoruz
        }



      /*  public IActionResult GetProducts()
{
    return Ok(_products); // 200 OK ile listeyi döndürür
}*/

        [HttpGet]
        public IActionResult GetProducts() 
        {
            if(_products==null)
            {
                return NotFound();
            }
          
            return Ok(_products);
        }

             //localhost:5001/api/products/1
             //[HttpGet("api/[controller]{id}")] böylede yazılabilir 
           [HttpGet("{id}")]
        public IActionResult GetProducts(int? id)
        {
            if(id==null)
            {
                // return StatusCode(404,"aradağınız kaynak bulunamadı");//daha kolay yolu var
                return NotFound();//404
            }
            var p= _products?.FirstOrDefault(x=>x.ProductId==id);
            if(p==null)
            {
                return NotFound();
            }
            return Ok(p);
        }
        
    }
    
 }