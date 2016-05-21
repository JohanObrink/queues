using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Redis;

namespace PaymentProcessor.Services
{
    public abstract class RedisStore<T> where T : class
    {
        private readonly IRedisClientsManager manager;
        private string name;

        protected RedisStore(IRedisClientsManager manager)
        {
            name = typeof(T).Name;
            this.manager = manager;
        }

        protected abstract string ResolveId(T data);

        public bool Set(T data, long? ttl = null)
        {
            var id = string.Format("{0}_{1}", name, ResolveId(data));
            try
            {
                using (var client = manager.GetClient())
                {
                    var result = client.Set<T>(id, data);
                    Console.WriteLine(string.Format("Stored {0} with id {1} in Redis", data.GetType().Name, id));
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed to store {0} with id {1} in Redis: {2}", data.GetType().Name, id, ex.Message));
                throw ex;
            }
        }

        public T Get(string id)
        {
            try
            {
                using (var client = manager.GetClient())
                {
                    return client.Get<T>(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed to retrieve {0} with id {1} in Redis: {2}", typeof(T).GetType().Name, id, ex.Message));
                throw ex;
            }
        }
    }
}
