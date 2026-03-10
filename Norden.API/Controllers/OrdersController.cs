using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Norden.API.Data;
using Norden.API.DTOs.Common;
using Norden.API.DTOs.Orders;
using Norden.API.Models;
using System.Security.Claims;

namespace Norden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(AppDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CreateOrderResponse>>> CreateOrder(CreateOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<CreateOrderResponse>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<CreateOrderResponse>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                // Get products and validate
                var productIds = request.Items.Select(i => Guid.Parse(i.ProductId)).ToList();
                var products = await _context.Products
                    .Include(p => p.Images)
                    .Where(p => productIds.Contains(p.Id) && p.IsActive)
                    .ToDictionaryAsync(p => p.Id, p => p);

                if (products.Count != productIds.Count)
                {
                    return BadRequest(ApiResponse<CreateOrderResponse>.ErrorResult("One or more products not found", "INVALID_PRODUCTS"));
                }

                // Create Address (using request payload info directly into a new Address record or looking for existing)
                var orderAddress = new Address
                {
                    UserId = userId.Value,
                    Label = "Order Shipping Address",
                    Name = "Order Recipient", // Or take from user account
                    Phone = "",
                    Street = request.ShippingAddress.Street,
                    City = request.ShippingAddress.City,
                    Country = request.ShippingAddress.Country,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                _context.Addresses.Add(orderAddress);

                // Calculate totals
                decimal subtotal = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in request.Items)
                {
                    var product = products[Guid.Parse(item.ProductId)];
                    var unitPrice = product.Price;
                    var totalPrice = unitPrice * item.Quantity;
                    subtotal += totalPrice;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ImageUrl = product.Images.OrderBy(i => i.SortOrder).FirstOrDefault()?.ImageUrl,
                        Quantity = item.Quantity,
                        Price = unitPrice,
                        SelectedColor = item.SelectedColor,
                        SelectedSize = item.SelectedSize
                    });
                }

                // Calculate shipping and tax (simplified)
                var shippingCost = subtotal > 100 ? 0 : 10; // Free shipping over $100
                var taxAmount = subtotal * 0.08m; // 8% tax
                var totalAmount = subtotal + shippingCost + taxAmount;

                // Create order
                var order = new Order
                {
                    UserId = userId.Value,
                    Address = orderAddress,
                    Status = "pending",
                    PaymentMethod = request.PaymentMethod,
                    SubTotal = subtotal,
                    Shipping = shippingCost,
                    Tax = taxAmount,
                    Total = totalAmount,
                    PromoCode = null,
                    Discount = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Add order items
                foreach (var item in orderItems)
                {
                    item.OrderId = order.Id;
                }

                _context.OrderItems.AddRange(orderItems);
                
                // Add initial tracking
                var tracking = new OrderTracking
                {
                    OrderId = order.Id,
                    Status = "Order Placed",
                    Description = "Your order has been placed and is being processed",
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrderTrackings.Add(tracking);

                await _context.SaveChangesAsync();

                var response = new CreateOrderResponse
                {
                    OrderId = order.Id.ToString(),
                    Status = order.Status,
                    TotalAmount = order.Total,
                    CreatedAt = order.CreatedAt
                };

                return Ok(ApiResponse<CreateOrderResponse>.SuccessResult(response, "Order created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, ApiResponse<CreateOrderResponse>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<OrderListDto>>> GetOrders([FromQuery] OrderQueryDto query)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<OrderListDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                var queryable = _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Where(o => o.UserId == userId.Value);

                if (!string.IsNullOrEmpty(query.Status))
                {
                    queryable = queryable.Where(o => o.Status == query.Status);
                }

                var totalItems = await queryable.CountAsync();

                var orders = await queryable
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((query.Page - 1) * query.Limit)
                    .Take(query.Limit)
                    .Select(o => new OrderDto
                    {
                        Id = o.Id.ToString(),
                        Status = o.Status,
                        TotalAmount = o.Total,
                        PaymentMethod = o.PaymentMethod,
                        Items = o.OrderItems.Select(oi => new OrderItemDto
                        {
                            ProductId = oi.Product.Id.ToString(),
                            ProductName = oi.ProductName,
                            ImageUrl = oi.ImageUrl,
                            Quantity = oi.Quantity,
                            Price = oi.Price,
                            SelectedColor = oi.SelectedColor,
                            SelectedSize = oi.SelectedSize
                        }).ToList(),
                        Address = new DTOs.Orders.AddressInfo
                        {
                            Street = o.Address != null ? o.Address.Street : string.Empty,
                            City = o.Address != null ? o.Address.City : string.Empty,
                            Country = o.Address != null ? o.Address.Country : string.Empty
                        },
                        CreatedAt = o.CreatedAt
                    })
                    .ToListAsync();

                var result = new OrderListDto
                {
                    Orders = orders,
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = query.Page,
                        TotalItems = totalItems,
                        ItemsPerPage = query.Limit,
                        TotalPages = (int)Math.Ceiling((double)totalItems / query.Limit)
                    }
                };

                return Ok(ApiResponse<OrderListDto>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders");
                return StatusCode(500, ApiResponse<OrderListDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(Guid orderId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<OrderDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId.Value);

                if (order == null)
                {
                    return NotFound(ApiResponse<OrderDto>.ErrorResult("Order not found", "NOT_FOUND"));
                }

                var orderDto = new OrderDto
                {
                    Id = order.Id.ToString(),
                    Status = order.Status,
                    TotalAmount = order.Total,
                    PaymentMethod = order.PaymentMethod,
                    Items = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.Product.Id.ToString(),
                        ProductName = oi.ProductName,
                        ImageUrl = oi.ImageUrl,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        SelectedColor = oi.SelectedColor,
                        SelectedSize = oi.SelectedSize
                    }).ToList(),
                    Address = new DTOs.Orders.AddressInfo
                    {
                        Street = order.Address != null ? order.Address.Street : string.Empty,
                        City = order.Address != null ? order.Address.City : string.Empty,
                        Country = order.Address != null ? order.Address.Country : string.Empty
                    },
                    CreatedAt = order.CreatedAt
                };

                return Ok(ApiResponse<OrderDto>.SuccessResult(orderDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", orderId);
                return StatusCode(500, ApiResponse<OrderDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpGet("{orderId}/tracking")]
        public async Task<ActionResult<ApiResponse<List<OrderTrackingDto>>>> GetOrderTracking(Guid orderId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<List<OrderTrackingDto>>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                // Verify order belongs to user
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId.Value);

                if (order == null)
                {
                    return NotFound(ApiResponse<List<OrderTrackingDto>>.ErrorResult("Order not found", "NOT_FOUND"));
                }

                var tracking = await _context.OrderTrackings
                    .Where(ot => ot.OrderId == orderId)
                    .OrderBy(ot => ot.CreatedAt)
                    .Select(ot => new OrderTrackingDto
                    {
                        Status = ot.Status,
                        Description = ot.Description,
                        Location = ot.Location,
                        CreatedAt = ot.CreatedAt
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<OrderTrackingDto>>.SuccessResult(tracking));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order tracking {OrderId}", orderId);
                return StatusCode(500, ApiResponse<List<OrderTrackingDto>>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
        }
    }
}
