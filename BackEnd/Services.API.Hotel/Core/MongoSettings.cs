namespace Services.API.Hotel.Core
{
    // MongoDB Settings for connect to the database
    public class MongoSettings
    {
        public required string ConnectionString { get; set; }
        public required string Database { get; set; }

    }
}
