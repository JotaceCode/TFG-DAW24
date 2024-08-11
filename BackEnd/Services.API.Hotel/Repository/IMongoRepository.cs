using Services.API.Hotel.Core.Dto;
using Services.API.Hotel.Core.Entities.PropertiesShared;

namespace Services.API.Hotel.Repository
{
    // Interface for Generic Methods for API (CRUD)
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {
        Task<IEnumerable<TDocument>> GetAll();

        Task<TDocument> GetById(string Id);

        Task InsertDocument(TDocument document);

        Task UpdateDocument(TDocument document);

        Task DeleteById(string Id);

        Task DeleteAll();

        Task<PaginationDto<TDocument>> PaginationBy(
            PaginationDto<TDocument> pagination
        );
    }
}
