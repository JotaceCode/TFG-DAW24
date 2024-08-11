
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Services.API.Hotel.Core;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;
using Services.API.Hotel.Core.Dto;
using Services.API.Hotel.Core.Entities.PropertiesShared;

namespace Services.API.Hotel.Repository
{
    // Class that implements Generic Methods for API (CRUD)
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {

        private readonly IMongoCollection<TDocument> _collection;

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()).CollectionName;
        }

        public MongoRepository(IOptions<MongoSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var db = client.GetDatabase(options.Value.Database);

            _collection = db.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        // Generic Methods for API (CRUD)
        public async Task<IEnumerable<TDocument>> GetAll()
        {
            return await _collection.Find(p => true).ToListAsync();
        }

        public async Task<TDocument> GetById(string Id)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, Id);
            return await _collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task InsertDocument(TDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task UpdateDocument(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filter, document);
        }

        public async Task DeleteById(string Id)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, Id);
            await _collection.FindOneAndDeleteAsync(filter);
        }

        public async Task DeleteAll()
        {
            var filter = Builders<TDocument>.Filter.Empty;
            await _collection.DeleteManyAsync(filter);
        }

        public async Task<PaginationDto<TDocument>> PaginationBy(PaginationDto<TDocument> pagination)
        {
            var sort = Builders<TDocument>.Sort.Ascending(pagination.Sort);

            if (pagination.SortDirection == "desc")
            {
                sort = Builders<TDocument>.Sort.Descending(pagination.Sort);
            }

            var filterBuilder = Builders<TDocument>.Filter;
            var totalDocuments = 0;
            FilterDefinition<TDocument> combinedFilter = filterBuilder.Empty;

            if (pagination.Filter != null && pagination.Filter.Any())
            {
                var filterDefinitions = pagination.Filter.Select(f =>
                {
                    // If is a date
                    if (f.Property == "CreatedDate" && DateTime.TryParse(f.Value, out DateTime date))
                    {
                        return filterBuilder.Gte(f.Property, date.Date) & filterBuilder.Lt(f.Property, date.Date.AddDays(1));
                    }
                    // If is a string and exactValues is true
                    else if (pagination.ExactValues.HasValue && pagination.ExactValues.Value)
                    {
                        return filterBuilder.Eq(f.Property, f.Value);
                    }
                    // Default case (use regex)
                    else
                    {
                        return filterBuilder.Regex(f.Property, new BsonRegularExpression(".*" + f.Value + ".*", "i"));
                    }
                }
                );
                combinedFilter = filterBuilder.Or(filterDefinitions);
            }

            if (pagination.Exclude.HasValue && pagination.Exclude.Value)
            {
                combinedFilter = filterBuilder.Not(combinedFilter);

            }

            pagination.Data = await _collection.Find(combinedFilter)
                .Sort(sort)
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Limit(pagination.PageSize)
                .ToListAsync();

            totalDocuments = (int)await _collection.Find(combinedFilter).CountDocumentsAsync();

            var rounded = Math.Ceiling(totalDocuments / Convert.ToDecimal(pagination.PageSize));
            var totalPages = Convert.ToInt32(rounded);

            pagination.PagesQuantity = totalPages;
            pagination.TotalRows = Convert.ToInt32(totalDocuments);

            return pagination;
        }

    }
}
