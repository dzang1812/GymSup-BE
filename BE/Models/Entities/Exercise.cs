using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE.Models.Entities;

public class Exercise
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }
    public List<string> TargetMuscles { get; set; }
    public string Equipment { get; set; }
    public string Difficulty { get; set; }

    public string ImageUrl { get; set; }
    public string VideoUrl { get; set; }
}
