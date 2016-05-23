using PaymentProcessor.Models;
using RethinkDb;

namespace PaymentProcessor.Services
{
    public class PaymentStore : RethinkDbTable<Payment>
    {
        public PaymentStore(IConnectionFactory connectionFactory, string dbName) : base(connectionFactory, dbName)
        {}
    }
}
