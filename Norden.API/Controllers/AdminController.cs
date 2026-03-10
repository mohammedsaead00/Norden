using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Norden.API.Data;
using Norden.API.DTOs.Admin;
using Norden.API.DTOs.Common;
using Norden.API.DTOs.Products;
using Norden.API.Models;
using System.Security.Claims;

namespace Norden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<ApiResponse<DashboardDto>>> GetDashboard()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                // Get overview statistics
                var totalOrders = await _context.Orders.CountAsync();
                var totalRevenue = await _context.Orders.SumAsync(o => o.Total);
                var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
                var totalUsers = await _context.Users.CountAsync(u => u.IsActive);

                var overview = new OverviewDto
                {
                    TotalOrders = totalOrders,
                    TotalRevenue = totalRevenue,
                    TotalProducts = totalProducts,
                    TotalUsers = totalUsers
                };

                // Get recent orders
                var recentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .Select(o => new OrderDto
                    {
                        Id = o.Id.ToString(),
                        OrderNumber = "ORD-" + o.Id.ToString().Substring(0, 8).ToUpper(),
                        Status = o.Status,
                        PaymentStatus = o.PaymentMethod,
                        TotalAmount = o.Total,
                        CreatedAt = o.CreatedAt
                    })
                    .ToListAsync();

                // Get top products
                var topProducts = await _context.OrderItems
                    .Include(oi => oi.Product)
                    .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                    .Select(g => new TopProductDto
                    {
                        ProductId = g.Key.ProductId.ToString(),
                        ProductName = g.Key.ProductName,
                        TotalSold = g.Sum(oi => oi.Quantity),
                        Revenue = g.Sum(oi => oi.Quantity * oi.Price)
                    })
                    .OrderByDescending(tp => tp.TotalSold)
                    .Take(10)
                    .ToListAsync();

                // Get sales chart data (last 5 months)
                var salesChart = new SalesChartDto
                {
                    Labels = new List<string>(),
                    Data = new List<decimal>()
                };

                for (int i = 4; i >= 0; i--)
                {
                    var date = DateTime.UtcNow.AddMonths(-i);
                    var monthName = date.ToString("MMM");
                    salesChart.Labels.Add(monthName);

                    var monthlyRevenue = await _context.Orders
                        .Where(o => o.CreatedAt.Month == date.Month && o.CreatedAt.Year == date.Year)
                        .SumAsync(o => o.Total);

                    salesChart.Data.Add(monthlyRevenue);
                }

                var dashboard = new DashboardDto
                {
                    Overview = overview,
                    RecentOrders = recentOrders,
                    TopProducts = topProducts,
                    SalesChart = salesChart
                };

                return Ok(ApiResponse<DashboardDto>.SuccessResult(dashboard));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin dashboard");
                return StatusCode(500, ApiResponse<DashboardDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpGet("products")]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var products = await _context.Products
                    .Include(p => p.Images)
                    .Include(p => p.Category)
                    .Include(p => p.Season)
                    .Include(p => p.Variants)
                    .Select(p => new ProductDto
                    {
                        Id = p.Id.ToString(),
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Category = p.Category != null ? p.Category.Name : null,
                        Season = p.Season != null ? p.Season.Slug : null,
                        Images = p.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl).ToList(),
                        Colors = p.Variants.Where(v => !string.IsNullOrEmpty(v.Color)).Select(v => v.Color!).Distinct().ToList(),
                        Sizes = p.Variants.Where(v => !string.IsNullOrEmpty(v.Size)).Select(v => v.Size!).Distinct().ToList(),
                        IsNew = p.IsNew,
                        IsFeatured = p.IsFeatured
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<ProductDto>>.SuccessResult(products));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin products");
                return StatusCode(500, ApiResponse<List<ProductDto>>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpGet("orders")]
        public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetOrders()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var orders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => new OrderDto
                    {
                        Id = o.Id.ToString(),
                        OrderNumber = "ORD-" + o.Id.ToString().Substring(0, 8).ToUpper(),
                        Status = o.Status,
                        PaymentStatus = o.PaymentMethod,
                        TotalAmount = o.Total,
                        CreatedAt = o.CreatedAt
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<OrderDto>>.SuccessResult(orders));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin orders");
                return StatusCode(500, ApiResponse<List<OrderDto>>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPut("orders/{orderId}/status")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateOrderStatus(Guid orderId, UpdateOrderStatusRequest request)
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Order not found", "NOT_FOUND"));
                }

                order.Status = request.Status;
                order.UpdatedAt = DateTime.UtcNow;

                // Add tracking entry
                var tracking = new OrderTracking
                {
                    OrderId = orderId,
                    Status = request.Status,
                    Description = $"Order status updated to {request.Status}",
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrderTrackings.Add(tracking);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResult(null, "Order status updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status {OrderId}", orderId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        private bool IsAdmin()
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            return isAdminClaim == "True";
        }
    }
}
