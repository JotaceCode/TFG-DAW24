using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Services.API.Hotel.Core.Entities.PropertiesShared
{
    // Class for implements in all Collection this attributes
    public class Document : IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("CreatedDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedDate { get; private set; }

        public Document()
        {
            if (CreatedDate == DateTime.MinValue)
            {
                CreatedDate = DateTime.UtcNow;
            }
        }
    }
}
