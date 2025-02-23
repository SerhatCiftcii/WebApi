using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
 {
    //localhost:5001/api/products
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private  readonly ProductsContext _context;
        public ProductsController(ProductsContext context)
        {
            _context = context;
        }
    


      /*  public IActionResult GetProducts()
{
    return Ok(_products); // 200 OK ile listeyi döndürür
}*/

        [HttpGet]
        public async Task <IActionResult> GetProducts() 
        {
           var products= await _context.Products.ToListAsync();
            return Ok(products);
          
            
        }

             //localhost:5001/api/products/1
             //[HttpGet("api/[controller]{id}")] böylede yazılabilir 
           [HttpGet("{id}")]
        public async Task<IActionResult> GetProducts(int? id)
        {
            if(id==null)
            {
                // return StatusCode(404,"aradağınız kaynak bulunamadı");//daha kolay yolu var
                return NotFound();//404
            }
            var p= await _context.Products.FirstOrDefaultAsync(x=>x.ProductId==id);
            if(p==null)
            {
                return NotFound();
            }
            return Ok(p);
        }
        
    }
    
 }