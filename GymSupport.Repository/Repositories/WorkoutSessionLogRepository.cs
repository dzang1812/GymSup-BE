using GymSupport.Repository.Interfaces;
using GymSupport.Repository.Models.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace GymSupport.Repository.Repositories;

public class WorkoutSessionLogRepository : IWorkoutSessionLogRepository
{
    private readonly IMongoCollection<WorkoutSessionLog> _collection;

    public WorkoutSessionLogRepository(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        _collection = database.GetCollection<WorkoutSessionLog>("WorkoutSessionLogs");
    }

    public async Task<WorkoutSessionLog> CreateAsync(WorkoutSessionLog sessionLog)
    {
        await _collection.InsertOneAsync(sessionLog);
        return sessionLog;
    }

    public async Task<WorkoutSessionLog?> GetByIdAsync(string id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<WorkoutSessionLog?> GetActiveByUserIdAsync(string userId)
    {
        return await _collection
            .Find(x =>
                x.UserId == userId &&
                (x.Status == "IN_PROGRESS" || x.Status == "PAUSED"))
            .FirstOrDefaultAsync();
    }

    public async Task<List<WorkoutSessionLog>> GetByUserIdAsync(string userId)
    {
        return await _collection
            .Find(x => x.UserId == userId)
            .SortByDescending(x => x.StartTime)
            .ToListAsync();
    }

    public async Task UpdateAsync(string id, WorkoutSessionLog sessionLog)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == id,
            sessionLog);
    }
}