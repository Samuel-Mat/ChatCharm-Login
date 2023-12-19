using MongoDB.Driver;
using MongoDbDriverWrapper.Demo.Db.Settings;

namespace ChatCharm_Login.Models
{
    public class ChatCharmDB : TypedMongoDbDriverWrapper.DbContext
    {
        public IMongoCollection<User> Users { get; set; }

        public ChatCharmDB(string dbName, string connectionString) :
            base(dbName, connectionString, new CollectionProvider())
        {
            Users = GetCollection<User>();
        }
    }
}
