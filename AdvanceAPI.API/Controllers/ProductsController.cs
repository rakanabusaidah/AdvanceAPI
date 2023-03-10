using AdvanceAPI.API.Models;
using AdvanceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdvanceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;

            _context.Database.EnsureCreated();
        }


        [HttpGet]
        public async Task<ActionResult> GetAllProducts([FromQuery] ProductQueryParameters? productQueryParameters)
        {
            IQueryable<Product> products = _context.Products;

            if (productQueryParameters?.MinPrice != null)
            {
                products = products.Where(
                    p => p.Price >= productQueryParameters.MinPrice.Value);
            }
            if (productQueryParameters?.MaxPrice != null)
            {
                products = products.Where(
                    p => p.Price <= productQueryParameters.MaxPrice.Value);
            }
            if (!string.IsNullOrEmpty(productQueryParameters?.Sku))
            {
                products = products.Where(
                    p => p.Sku.ToLower() == productQueryParameters.Sku.ToLower());
            }
            if (!string.IsNullOrEmpty(productQueryParameters?.Name))
            {
                products = products.Where(
                    p => p.Name.ToLower().Contains(productQueryParameters.Name.ToLower()));
            }
            if(!string.IsNullOrEmpty(productQueryParameters?.SearchTerm))
            {
                products = products.Where(
                    p => p.Sku.ToLower().Contains(productQueryParameters.Sku.ToLower())
                    || p.Name.ToLower().Contains(productQueryParameters.Name.ToLower())
                    );
            }
            if(!string.IsNullOrEmpty(productQueryParameters?.SortBy))
            {
                // to check of the property is correct and exists in product
                if(typeof(Product).GetProperty(productQueryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(
                        productQueryParameters.SortBy,
                        productQueryParameters.SortOrder
                        );
                }
            }
            if (productQueryParameters != null)
            {

                products = products
                    .Skip(productQueryParameters.Size * (productQueryParameters.Page - 1))
                    .Take(productQueryParameters.Size);

                return Ok(await products.ToArrayAsync());
            }
            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            /*
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            */
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult> DeleteMultiple([FromQuery] int[] ids)
        {
            var products = new List<Product>();
            foreach (var id in ids)
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                products.Add(product);
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok(products);
        }
    }
}
