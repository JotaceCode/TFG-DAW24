using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Services.API.Hotel.Core.Entities.PropertiesShared;
using System.ComponentModel.DataAnnotations;

namespace Services.API.Hotel.Core.Entities
{
    // Define a 'Room' Model
    // IN the DB : db.Room.createIndex({ "RoomNumber": 1 }, { unique: true })
    [BsonCollectionAttribute("Room")]
    public class RoomEntity : Document
    {
        [BsonElement("RoomNumber")]
        public required string RoomNumber { get; set; }

        [BsonElement("Floor")]
        public string? Floor { get; set; }

        [BsonElement("Type")]
        public string? Type { get; set; }

        [BsonElement("Status")]
        public string? Status { get; set; }
    }
}
