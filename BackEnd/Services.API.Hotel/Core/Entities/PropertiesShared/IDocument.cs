using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Services.API.Hotel.Core.Entities.PropertiesShared
{
    // Interface for impelements in all Collection this attributes
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string? Id { get; set; }
    }
}
