using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Models
{
    public class Payment
    {
        public Payment()
        {
            Id = Guid.NewGuid().ToString();
            Status = PaymentStatus.Pending;
        }

        public string Id { get; set; }
        public int Amount { get; set; }
        public string OrderNumber { get; set; }
        public DateTime SentForPayment { get; set; }
        public DateTime? ResponseRecieved { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
