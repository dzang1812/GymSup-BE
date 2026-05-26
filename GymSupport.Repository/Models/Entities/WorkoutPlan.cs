using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE.Models.Entities;

public class WorkoutPlan
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Goal { get; set; }
    public int DaysPerWeek { get; set; }

    public List<WorkoutSession> Sessions { get; set; }
}
