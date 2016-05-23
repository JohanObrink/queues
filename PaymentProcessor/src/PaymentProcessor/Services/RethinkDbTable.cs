using RethinkDb;
using RethinkDb.DatumConverters;
using RethinkDb.Expressions;
using RethinkDb.Newtonsoft.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentProcessor.Services
{
    public abstract class RethinkDbTable<T>
    {
        private static QueryConverter Converter = new QueryConverter(new AggregateDatumConverterFactory(
            ArrayDatumConverterFactory.Instance,
            BinaryDatumConverterFactory.Instance,
            BoundEnumDatumConverterFactory.Instance,
            CompoundIndexDatumConverterFactory.Instance,
            DataContractDatumConverterFactory.Instance,
            DateTimeDatumConverterFactory.Instance,
            DateTimeOffsetDatumConverterFactory.Instance,
            EnumDatumConverterFactory.Instance,
            GroupingDictionaryDatumConverterFactory.Instance,
            GuidDatumConverterFactory.Instance,
            ListDatumConverterFactory.Instance,
            NamedValueDictionaryDatumConverterFactory.Instance,
            NullableDatumConverterFactory.Instance,
            ObjectDatumConverterFactory.Instance,
            PrimitiveDatumConverterFactory.Instance,
            TimeSpanDatumConverterFactory.Instance,
            TupleDatumConverterFactory.Instance,
            UriDatumConverterFactory.Instance,
            NewtonsoftDatumConverterFactory.Instance
        ), new DefaultExpressionConverterFactory());

        private readonly string dbName;
        private readonly string tableName;
        private readonly IConnectionFactory connectionFactory;
        private readonly IDatabaseQuery db; // = Query.Db("test");
        private readonly ITableQuery<T> table;

        private bool dbExists = false;
        private bool tableExists = false;

        public RethinkDbTable(IConnectionFactory connectionFactory, string dbName, string tableName = null)
        {
            this.dbName = dbName;
            this.tableName = tableName ?? typeof(T).Name + "s";
            this.db = Query.Db(this.dbName);
            this.connectionFactory = connectionFactory;
            this.table = db.Table<T>(this.tableName);
        }

        private IConnection Connect()
        {
            var conn = connectionFactory.Get();

            // create db if needed
            if (!dbExists)
            {
                if (!conn.Run(Query.DbList()).Contains(dbName))
                {
                    conn.Run(Query.DbCreate(dbName));
                }
                dbExists = true;
            }

            // Create table if needed
            if (!tableExists)
            {
                if (!conn.Run(db.TableList()).Contains(tableName))
                {
                    conn.Run(db.TableCreate(tableName));
                }
                tableExists = true;
            }

            return conn;
        }

        public T Get(string id)
        {
            using (var conn = Connect())
            {
                return conn.Run(table.Get(id), Converter);
            }
        }

        public void Set(T data)
        {
            using (var conn = Connect())
            {
                conn.Run(table.Insert(data), Converter);
            }
        }

        public void Update(Func<T, string> Id, T data)
        {
            using (var conn = Connect())
            {
                conn.Run(table.Get(Id(data)).Update(d => data), Converter);
            }
        }

        public void Delete(string id)
        {
            using(var conn = Connect())
            {
                conn.Run(table.Get(id).Delete(), Converter);
            }
        }

        public IEnumerable<T> List()
        {
            using (var conn = Connect())
            {
                return conn.Run(table, Converter).ToList();
            }
        }
    }
}
