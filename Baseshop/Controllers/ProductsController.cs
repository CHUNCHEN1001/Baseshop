using Baseshop.Dtos;
using Baseshop.Enums;
using Baseshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Baseshop.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly WebContext _context;
        private readonly string _connectionString;

        public ProductsController(WebContext context)
        {
            _context = context;
            _connectionString = _context.Database.GetDbConnection().ConnectionString;
        }

        // GET: api/products
        [HttpGet]
        public IActionResult GetProducts([FromQuery] string? productId, [FromQuery] string? name, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            List<ProductsDto> productDtos = new List<ProductsDto>();

            bool isSearch = !string.IsNullOrEmpty(productId) || !string.IsNullOrEmpty(name) || startDate.HasValue || endDate.HasValue;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                if (isSearch)
                {
                    string sql = "SELECT * FROM Products WHERE 1=1";
                    if (!string.IsNullOrEmpty(productId)) sql += " AND ProductId = @pid";
                    if (!string.IsNullOrEmpty(name)) sql += " AND Name LIKE @name";
                    if (startDate.HasValue) sql += " AND LastUpdatedTime >= @start";
                    if (endDate.HasValue) sql += " AND LastUpdatedTime < @end";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", productId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@name", string.IsNullOrEmpty(name) ? (object)DBNull.Value : $"%{name}%");
                        cmd.Parameters.AddWithValue("@start", startDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@end", endDate?.AddDays(1) ?? (object)DBNull.Value);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productDtos.Add(new ProductsDto
                                {
                                    ProductId = reader["ProductId"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    Category = reader["Category"].ToString(),
                                    Status = (ProductStatus)Convert.ToInt32(reader["Status"]),
                                    StockQuantity = Convert.ToInt32(reader["StockQuantity"]),    
                                    ImagePath = reader["ImagePath"].ToString(),
                                    LastUpdatedTime = reader["LastUpdatedTime"] as DateTime?,
                                    LastUpdatedBy = reader["LastUpdatedBy"]?.ToString()
                                });
                            }
                        }
                    }
                }
                else
                {
                    string sql = "SELECT * FROM Products";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            productDtos.Add(new ProductsDto
                            {
                                ProductId = row["ProductId"].ToString(),
                                Name = row["Name"].ToString(),
                                Category = row["Category"].ToString(),
                                Status = (ProductStatus)Convert.ToInt32(row["Status"]),
                                StockQuantity = Convert.ToInt32(row["StockQuantity"]),
                                ImagePath = row["ImagePath"].ToString(),
                                LastUpdatedTime = row["LastUpdatedTime"] == DBNull.Value ? null : (DateTime?)row["LastUpdatedTime"],
                                LastUpdatedBy = row["LastUpdatedBy"]?.ToString()
                            });
                        }
                    }
                }
            }
            return Ok(productDtos);
        }

        // GET: api/products/5
        [Authorize(Roles = "System,Admin,Store")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDetails(string id)
        {
            if (id == null)
            {
                return BadRequest("商品識別碼不能為空。"); ;
            }

            var product = await (from a in _context.Products
                                 where a.ProductId == id
                                 select new ProductsDetailsDto
                                 {
                                     Name = a.Name,
                                     Category = a.Category,
                                     StockQuantity = a.StockQuantity,
                                     Status = a.Status,
                                     ImagePath = a.ImagePath,
                                     CreatedTime = a.CreatedTime,
                                     LastUpdatedBy = a.LastUpdatedBy,
                                     LastUpdatedTime = a.LastUpdatedTime
                                 }).SingleOrDefaultAsync();

            if (product == null)
            {
                return NotFound($"找不到識別碼為 {id} 的商品。");
            }

            return Ok(product);
        }

        // GET: Products/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: api/products
        [HttpPost]
        [Authorize(Roles = "System,Admin,Store")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductsCreateDto product)
        {
                Product insert = new Product()
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Category = product.Category,
                    StockQuantity = product.StockQuantity,
                    Status = product.Status,
                    ImagePath = product.ImagePath
                };
                _context.Add(insert);
                await _context.SaveChangesAsync();

                //return RedirectToAction(nameof(Index));
                return CreatedAtAction(nameof(GetProductDetails), new { id = insert.ProductId }, insert);
        }

        //// GET: Products/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await (from a in _context.Products
        //                  where a.ProductId == id
        //                  select new ProductsEditDto
        //                  {
        //                      ProductId = a.ProductId,
        //                      Name = a.Name,
        //                      Category = a.Category,
        //                      StockQuantity= a.StockQuantity,
        //                      Status= a.Status,
        //                      ImagePath= a.ImagePath
        //                  }).SingleOrDefaultAsync();



        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //// POST: Products/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, ProductsEditDto productDto)
        //{
        //    if (id != productDto.ProductId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var product = await _context.Products.FindAsync(id);

        //            if (product == null)
        //            {
        //                return NotFound();
        //            }

        //            product.Name = productDto.Name;
        //            product.Category = productDto.Category;
        //            product.StockQuantity = productDto.StockQuantity;
        //            product.Status = productDto.Status; 
        //            product.ImagePath = productDto.ImagePath;
        //            product.LastUpdatedTime = DateTime.Now;
        //            product.LastUpdatedBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(productDto.ProductId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(productDto);
        //}

        // PUT: api/products/5
        [HttpPut("{id}")]
        [Authorize(Roles = "System,Admin,Store")]
        public async Task<IActionResult> EditProduct(string id, [FromBody] ProductsEditDto productDto)
        {
            if (id != productDto.ProductId)
            {
                return BadRequest("網址代碼與資料內容的商品識別碼不符。"); 
            }

            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound($"找不到識別碼為 {id} 的商品。");
                }

                product.Name = productDto.Name;
                product.Category = productDto.Category;
                product.StockQuantity = productDto.StockQuantity;
                product.Status = productDto.Status;
                product.ImagePath = productDto.ImagePath;
                product.LastUpdatedTime = DateTime.Now;
                product.LastUpdatedBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(productDto.ProductId))
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

        private bool ProductExists(string id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        //// GET: Products/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await (from a in _context.Products
        //                         where a.ProductId == id
        //                         select new ProductsDto
        //                         {
        //                             ProductId = a.ProductId,
        //                             Name = a.Name,
        //                             Category = a.Category,
        //                             StockQuantity = a.StockQuantity,
        //                             Status = a.Status,
        //                             ImagePath = a.ImagePath,
        //                             CreatedTime = a.CreatedTime,
        //                             LastUpdatedBy = a.LastUpdatedBy,
        //                             LastUpdatedTime = a.LastUpdatedTime
        //                         }).SingleOrDefaultAsync();

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        // POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var product = await _context.Products.FindAsync(id);
        //    if (product != null)
        //    {
        //        _context.Products.Remove(product);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "System,Admin,Store")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound($"找不到識別碼為 {id} 的商品，無法刪除。");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // 刪除成功通常回傳 204 No Content
            return NoContent();
        }
    }
}
