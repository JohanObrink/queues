using PaymentProcessor.Models;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Services
{
    public class PaymentStore : ManagedList<Payment>
    {
        public PaymentStore(IRedisClientsManager manager) : base(manager, "payments")
        {}

        public PaymentStore Update(Payment p)
        {
            this.Remove(p);
            this.Add(p);
            return this;
        }
    }
}
