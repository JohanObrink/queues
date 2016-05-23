using Newtonsoft.Json;
using RethinkDb.Spec;
using System;

namespace PaymentProcessor.Models
{
    public class Payment
    {
        public Payment()
        {
            Id = Guid.NewGuid().ToString();
            Status = PaymentStatus.Pending;
        }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        public int Amount { get; set; }
        public string OrderNumber { get; set; }
        public DateTime SentForPayment { get; set; }
        public DateTime? ResponseRecieved { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
