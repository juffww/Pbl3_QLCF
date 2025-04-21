namespace pbl3_QLCF.ViewModels
{
    public class CTKHViewModel
    {
        // Customer Info
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string SDT { get; set; }
        public int? LoyaltyPoints { get; set; }
        public string CustomerType { get; set; }

        // Summary Info
        public int TotalSpent { get; set; }
        public int OrderCount { get; set; }
        public double AverageOrderValue { get; set; }
        public DateTime? LastPurchaseDate { get; set; }

        // Order History
        public List<OrderSummary> Orders { get; set; }

        public class OrderSummary
        {
            public string OrderId { get; set; }
            public DateTime OrderTime { get; set; }
            public int? TotalAmount { get; set; }
        }
    }
}
