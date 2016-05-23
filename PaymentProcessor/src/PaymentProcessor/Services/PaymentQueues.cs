using Newtonsoft.Json.Linq;
using PaymentProcessor.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PaymentProcessor.Services
{
    public class PaymentQueues
    {
        private readonly IConnectionFactory rabbitMQConnectionFactory;
        private readonly PaymentStore store;
        private IConnection connection = null;
        private IModel channel = null;
        private EventingBasicConsumer consumer = null;
        private int retries = 0;
        private Queue<Payment> pending = new Queue<Payment>();

        private static readonly string INCOMING = "payments";
        private static readonly string SUCCESSFUL = "successful-payments";
        private static readonly string FAILED = "failed-payments";

        public PaymentQueues(IConnectionFactory rabbitMQConnectionFactory, PaymentStore store)
        {
            this.rabbitMQConnectionFactory = rabbitMQConnectionFactory;
            this.store = store;
        }

        private IModel CreateQueue(string name)
        {
            var queue = connection.CreateModel();
            queue.QueueDeclare(queue: name, durable: false, exclusive: false, autoDelete: false, arguments: null);
            return queue;
        }

        public void Connect()
        {
            if (connection == null || !connection.IsOpen)
            {
                try
                {
                    Console.WriteLine("Connecting to RabbitMQ, attempt: " + (++retries));
                    connection = rabbitMQConnectionFactory.CreateConnection();

                    channel = connection.CreateModel();
                    channel.QueueDeclare(queue: INCOMING, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: SUCCESSFUL, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: FAILED, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    consumer = new EventingBasicConsumer(channel);
                    consumer.Registered += Consumer_Registered;
                    consumer.Received += Consumer_Received;
                    channel.BasicConsume("payments", false, consumer);

                    retries = 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection failed: " + ex.Message);
                    Thread.Sleep(2000);
                    Connect();
                    return;
                }
            }
            SendPending();
        }

        private void Consumer_Registered(object sender, ConsumerEventArgs e)
        {
            Console.WriteLine("Consumer registered: " + e.ConsumerTag);
        }
        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey != INCOMING) return;
            var json = Encoding.UTF8.GetString(e.Body);
            var payment = JObject.Parse(json).ToObject<Payment>();

            // send to some service
            payment.SentForPayment = DateTime.Now;
            store.Set(payment);

            channel.BasicAck(e.DeliveryTag, false);
        }

        public void HandleResponse(string id, bool success)
        {
            var payment = store.Get(id);
            payment.ResponseRecieved = DateTime.Now;
            payment.Status = (success) ? PaymentStatus.Success : PaymentStatus.Fail;
            store.Update(p => p.Id, payment);
            Send(payment);
        }

        public void Send(Payment payment)
        {
            pending.Enqueue(payment);
            Connect();
        }

        public void SendPending()
        {
            while(pending.Count > 0)
            {
                var payment = pending.Dequeue();
                var message = JObject.FromObject(payment).ToString();
                var body = Encoding.UTF8.GetBytes(message);
                var type = payment.Status == PaymentStatus.Success ? SUCCESSFUL : FAILED;
                channel.BasicPublish(exchange: "", routingKey: type, basicProperties: null, body: body);
            }
        }
    }
}
