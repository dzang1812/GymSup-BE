using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE.Models.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string FullName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public string Role { get; set; } // Admin, Manager, Customer

    public string GymId { get; set; } // null nếu là Admin
}
