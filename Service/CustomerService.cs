
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using pbl3_QLCF.Data;
namespace pbl3_QLCF.Service
{
    public class CustomerService
    {
        private readonly Pbl3Context _context;
        private const int vip_min = 800000;
        private const int regular_min = 500000;
        private const int new_min = 100000;
        public CustomerService(Pbl3Context context) 
        {
            _context = context;
        }
        public string CustomerType(string customerId)
        {
            DateTime startDate = DateTime.Now.AddDays(-90);
            var orders = _context.DonHangs.Where(o => o.MaKh == customerId && o.ThoiGianDat >= startDate)
                        .ToList();
            if(!orders.Any())
            {
                return "Không hoạt động";
            }
            double totalSpending = orders.Sum(o => o.TongTien ?? 0);
            if(totalSpending >= vip_min)
            {
                return "VIP";
            }
            else if(totalSpending >= regular_min)
            {
                return "Thường xuyên";
            }
            else if(totalSpending >= new_min)
            {
                return "Mới";
            }    
            else 
            {
                return "Không hoạt động";
            }
        }
        public void UpdateCustomerTypes()
        {
            var date_min = DateTime.Now.AddDays(-90);

            var customerIds = _context.DonHangs.Where(o => o.ThoiGianDat >= date_min)
                               .Select(o => o.MaKh)
                               .Distinct()
                               .ToList();
            var potentialCustomer = _context.KhachHangs.Where(c => c.LoaiKH != "Không hoạt động" && !customerIds.Contains(c.MaKh))
                               .Select(c => c.MaKh)
                               .Distinct()
                               .ToList();
            var customerUpdate = customerIds.Union(potentialCustomer).Distinct().ToList();
            foreach (var customerId in customerUpdate)
            {
                var customer = _context.KhachHangs.Find(customerId);
                if (customer != null)
                {
                    string newType = CustomerType(customerId);
                    if(customer.LoaiKH != newType)
                    {
                        customer.LoaiKH = newType;
                    }
                }
            }
            _context.SaveChanges();
        }
    }
}
