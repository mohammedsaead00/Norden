using Norden.API.DTOs.Common;

namespace Norden.API.DTOs.Admin
{
    public class DashboardDto
    {
        public OverviewDto Overview { get; set; } = new OverviewDto();
        public List<OrderDto> RecentOrders { get; set; } = new List<OrderDto>();
        public List<TopProductDto> TopProducts { get; set; } = new List<TopProductDto>();
        public SalesChartDto SalesChart { get; set; } = new SalesChartDto();
    }

    public class OverviewDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
    }

    public class TopProductDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class SalesChartDto
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> Data { get; set; } = new List<decimal>();
    }

    public class OrderDto
    {
        public string Id { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
