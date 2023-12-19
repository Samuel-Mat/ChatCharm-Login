using System;
using System.Collections.Generic;
using System.Linq;
using TypedMongoDbDriverWrapper;
using ChatCharm_Login.Models;

namespace MongoDbDriverWrapper.Demo.Db.Settings
{
    public record DocumentCollection(Type DocumentType, string CollectionName) : IDocumentCollection;
    public class CollectionProvider : ICollectionProvider
    {
        public ICollection<IDocumentCollection> GetAll()
        {
            return new List<IDocumentCollection>
            {
                new DocumentCollection(typeof(User), "User"),
            };
        }
    }
}
