using MongoDbDriverWrapper.Demo.Db.Documents;
using System.ComponentModel.DataAnnotations;

namespace ChatCharm_Login.Models
{
    public class User : BaseDocument
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public byte[] Image { get; set; }

    }
}
