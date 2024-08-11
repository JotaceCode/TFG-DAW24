using Services.API.Security.Core.Entities;

namespace Services.API.Security.Core.Dto
{
    public class UsersFilteredDto
    {
        public required int PageSize { get; set; }
        public required int Page { get; set; }
        public required string Sort { get; set; }
        public required string SortDirection { get; set; }
        public FilterParameters? Filter { get; set; }
        public int PagesQuantity { get; set; }
        public IEnumerable<RegisteredUserDto>? Data { get; set; }
        public int TotalRows { get; set; }
    }
}
