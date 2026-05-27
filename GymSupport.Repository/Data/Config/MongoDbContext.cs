using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
namespace GymCoach.Api.Config;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration config)
    {
        var connectionString = config["MongoDbSettings:ConnectionString"];
        var dbName = config["MongoDbSettings:DatabaseName"];

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(dbName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
        => _database.GetCollection<T>(name);
}
