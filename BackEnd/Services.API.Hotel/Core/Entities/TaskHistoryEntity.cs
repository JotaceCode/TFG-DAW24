using MongoDB.Bson.Serialization.Attributes;
using Services.API.Hotel.Core.Dto;
using Services.API.Hotel.Core.Entities.PropertiesShared;
using System.ComponentModel.DataAnnotations;

namespace Services.API.Hotel.Core.Entities
{
    // Define a 'Task' Model
    // IN the DB : db.Task.createIndex({ "Room.RoomNumber": 1 }, { unique: true })
    [BsonCollectionAttribute("TaskHistory")]
    public class TaskHistoryEntity : Document
    {
        [BsonElement("Room")]
        public required RoomEntity Room { get; set; }

        [BsonElement("User")]
        public required RegisteredUserDto User { get; set; }

        [BsonElement("Priority")]
        public string? Priority { get; set; }

        [BsonElement("Observations")]
        public string? Observations { get; set; }

        [BsonElement("Status")]
        public string? Status { get; set; }
    }
}
