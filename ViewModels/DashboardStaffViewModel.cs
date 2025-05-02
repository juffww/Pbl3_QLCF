namespace pbl3_QLCF.ViewModels
{
    public class DashboardStaffViewModel
    {
        public int? todayOrder {  get; set; }
        public int? orderCompleted {  get; set; }
        public int? proOrderCount { get; set; }
        public List<OrderInProcessing> processOrders { get; set; }
        public DashboardStaffViewModel() { 
            processOrders = new List<OrderInProcessing>();   
        }
    }
    public class orderItem
    {
        public string productName { get; set; }
        public int quantity { get; set; }
    }
    public class OrderInProcessing
    {
        public string orderId { get; set; }
        public string customerName { get; set; }
        public List<orderItem> items { get; set; }
        public DateTime orderTime { get; set; }
        public String status { get; set; }
    }
}
