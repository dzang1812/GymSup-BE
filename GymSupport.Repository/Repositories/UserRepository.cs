using GymSupport.Repository.Models.Entities;
using GymCoach.Api.Config;
using MongoDB.Driver;
using System.Threading.Tasks;
using GymSupport.Repository.Interfaces;

namespace GymSupport.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbContext context)
        {
            _users = context.GetCollection<User>("Users");
            var emailIndex = Builders<User>.IndexKeys.Ascending(u => u.Email);
            _users.Indexes.CreateOne(new CreateIndexModel<User>(emailIndex, new CreateIndexOptions { Unique = true }));
        }

        public Task CreateAsync(User user) => _users.InsertOneAsync(user);

        public Task UpdateAsync(User user) =>
            _users.ReplaceOneAsync(u => u.Id == user.Id, user);

        public Task<User?> GetByEmailAsync(string email) =>
            _users.Find(u => u.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();

        public Task<User?> GetByIdAsync(string id) =>
            _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }
}
