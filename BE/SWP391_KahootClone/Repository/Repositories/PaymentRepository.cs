using Microsoft.EntityFrameworkCore;
using Repository.Base;
using Repository.DBContext;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class PaymentRepository : GenericRepository<Payment> {
        public PaymentRepository() { }

        public PaymentRepository(SWP_KahootContext context) => _context = context;
        public async Task<Payment> GetPaymentByPaymentMethod(string paymentMethod)
        {
            return await _context.Payments.Where(x => x.PaymentMethod == paymentMethod).FirstOrDefaultAsync();
        }
    }
}
