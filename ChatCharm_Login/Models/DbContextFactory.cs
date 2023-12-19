using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDbDriverWrapper.Demo.Db.Settings;
using TypedMongoDbDriverWrapper;
using ChatCharm_Login.Models;

namespace MongoDbDriverWrapper.Demo.Db.DbContext
{
    public static class DbContextFactory
    {
        public record BsonSerializer(Type Type, IBsonSerializer Serializer) : IBsonSerialization;
        public static async Task<ChatCharmDB> CreateAsync(string dbName, string connectionString)
        {
            //Register Serializers first (if any)
            TypedMongoDbDriverWrapper.DbContext.RegisterSerializers(new List<IBsonSerialization>
            {
                new BsonSerializer(typeof(DateTimeOffset),new DateTimeOffsetSerializer(BsonType.Document))
            });

            //Initialize DbContext
            var dbContext = new ChatCharmDB(dbName, connectionString);

            //Create Collections
            await dbContext.CreateCollectionsAsync();

         

            return dbContext;
        }
    }
}