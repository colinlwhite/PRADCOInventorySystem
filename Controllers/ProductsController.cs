using Microsoft.AspNetCore.Mvc;
using Dapper;
using PRADCOInventorySystem.DAL;
using PRADCOInventorySystem.Models;
using System.Data;

namespace PRADCOInventorySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly DapperContext _context;

        public ProductsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var query = "SELECT * FROM Products";
            using (var connection = _context.CreateConnection())
            {
                var products = await connection.QueryAsync<Product>(query);
                return Ok(products);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var query = "SELECT * FROM Products WHERE ProductID = @Id";
            using (var connection = _context.CreateConnection())
            {
                var product = await connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });

                if (product == null)
                    return NotFound();

                return Ok(product);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            var query = "INSERT INTO Products (Name, BrandID, SKU, Category, Price, StockQuantity) " +
                        "VALUES (@Name, @BrandID, @SKU, @Category, @Price, @StockQuantity)";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, product);
                return CreatedAtAction(nameof(GetProducts), new { id = product.ProductID }, product);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.ProductID)
                return BadRequest();

            var query = "UPDATE Products SET Name = @Name, BrandID = @BrandID, SKU = @SKU, " +
                        "Category = @Category, Price = @Price, StockQuantity = @StockQuantity " +
                        "WHERE ProductID = @ProductID";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, product);
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var query = "DELETE FROM Products WHERE ProductID = @ProductID";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { ProductID = id });
                return NoContent();
            }
        }
    }
}
