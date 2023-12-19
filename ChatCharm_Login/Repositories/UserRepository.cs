using ChatCharm_Login.Models;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using TypedMongoDbDriverWrapper;



namespace ChatCharm_Login.Repositories
{

    

    public class UserRepository : BaseRepository<User>
    {
        private readonly ILogger<UserRepository> _logger;

        private readonly ChatCharmDB _dbContext;

        public UserRepository(ChatCharmDB dbContext, ILogger<UserRepository> logger) : base(dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<User> GetById(string id)
        {
            return await GetSingleOrThrowAsync(id);
        }

        public async Task<User> GetByName(string username)
        {
            User user = await _dbContext.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
            Console.WriteLine("User:" + user);
            return user;
        }


        public async Task DeleteUser(string id)
        {
            await DeleteAsync(id);
        }

        public async Task ChangePassword(string id, string password)
        {
            User user = await GetSingleOrThrowAsync(id);
            user.Password = password;
        }
    }
}
