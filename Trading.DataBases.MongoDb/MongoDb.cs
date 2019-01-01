using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataBases.Interfaces;

namespace Trading.DataBases.MongoDb
{
    public class MongoDb : IDataBase
    {
        IMongoClient client;

        public MongoDb()
        {
            //client = new Lazy<IMongoClient>(() =>
            //{
            //    return new MongoClient("mongodb://localhost:27017");
            //}).Value;

            client = new MongoClient("mongodb://localhost:27017");
        }

        private IMongoDatabase getDB(InstrumentType instrumentType)
        {
            return client.GetDatabase(instrumentType.ToString());
        }

        private string createCollectionName(Instrument instrument, Resolution resolution)
        {
            return $"{instrument.Name}-{resolution.TimeFrame}-{resolution.Size}";
        }
        private IMongoCollection<Bar> getCollection(Instrument instrument, Resolution resolution)
        {
            var db = client.GetDatabase(instrument.Type.ToString());
            var collectionName = createCollectionName(instrument, resolution);
            var collection = db.GetCollection<Bar>(collectionName);
            return collection;
        }

        public void WriteLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            var col = getCollection(instrument, resolution);

            foreach (var b in barList)
            {
                col.InsertOne(b);
            }
        }

        public IEnumerable<Bar> ReadLocalData(Instrument instrument, Resolution resolution, DateTime fromDate, DateTime toDate)
        {
            var collection = getCollection(instrument, resolution);

            var list = collection.Find(new BsonDocument()).ToList();

            return list;
        }

        public void AppendLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            throw new NotImplementedException();
        }

        public void PrependLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            throw new NotImplementedException();
        }

    }
}
