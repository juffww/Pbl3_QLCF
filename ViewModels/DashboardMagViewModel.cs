namespace pbl3_QLCF.ViewModels
{
    public class DashboardMagViewModel
    {
        public int todayRevenue {  get; set; }
        public double todayPercent {  get; set; }
        public int todayOrder {  get; set; }
        public double orderPercent {  get; set; }
        public int newCustomerCount { get; set; }
        public double customerPercent { get; set; }
        public List<revenueTrendItem> revenueTrend {  get; set; }
        public List<topSelling> topSellingProduct {  get; set; }
        public List<recentOrder> recentOrders { get; set; }
    }
    public class revenueTrendItem
    {
        public string time {  get; set; }
        public int revenue {  get; set; }
    }
    public class topSelling
    {
        public string productName { get; set; }
        public int quantity { get; set; }
    }
    public class recentOrder
    {
        public string orderID {  get; set; }
        public string customerName { get; set; }
        public int tongTien {  get; set; }
        public string status {  get; set; }
    }
}
