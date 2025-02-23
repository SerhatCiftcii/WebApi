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
        //veri ekleme
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product entity)
        {
          _context.Products.Add(entity);
          await _context.SaveChangesAsync();
          // ilk olarak hangi action methoda giticek sonra , ile new yazarak id bilgisini gönderiyoruz idye karşılık değer gelende entytyProdcutId olacak son parametre ise hangi entity eklendi o.
          return CreatedAtAction(nameof(GetProducts),new {id=entity.ProductId},entity);
        }
        //veri güncelleme
        [HttpPut("{id}")]
        public async Task <IActionResult> UpdateProduct(int id,Product entity)
    {
            
            if(id!=entity.ProductId)
            {
                return BadRequest(); //400 
            }
            var product= await _context.Products.FirstOrDefaultAsync(x=>x.ProductId==id);
            if(product==null)
            {
                return NotFound();//404 
            }
            product.ProductName=entity.ProductName;
            product.Price=entity.Price;
            product.IsActive=entity.IsActive;
            ;
            try 
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }
            return NoContent();//204 herşey normal geriye birşey döndürmüyoruz günceleme işlemi başarılı
    }
    
    /// <summary>
    /// Ürün Silme İşlemi
    /// </summary>
    /// <param name="id">Silinecek Ürün Idsi</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int? id)
    {
        if(id==null)
        {
            return NotFound();
        }
        var product= await _context.Products.FirstOrDefaultAsync(x=>x.ProductId==id);
        if(product==null)
        {
            return NotFound();
        }
        _context.Products.Remove(product);
        try{
            await _context.SaveChangesAsync();
        }
        catch(Exception)
        {
            return NotFound();
        }
        return Ok(new{message="Ürün başarıyla silindi",deletedProductId=product});
    }
    
 }
 }