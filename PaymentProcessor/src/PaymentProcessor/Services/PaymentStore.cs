using PaymentProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RethinkDb;

namespace PaymentProcessor.Services
{
    public class PaymentStore : RethinkDbTable<Payment>
    {
        public PaymentStore(IConnectionFactory connectionFactory, string dbName) : base(connectionFactory, dbName)
        {}
    }
}
