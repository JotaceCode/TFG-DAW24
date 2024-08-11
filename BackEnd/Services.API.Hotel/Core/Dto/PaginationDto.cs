namespace Services.API.Hotel.Core.Dto
{
    // Helped class for data persistence when paginate
    public class PaginationDto<TDocument>
    {
        public required int PageSize { get; set; }
        public required int Page { get; set; }
        public required string Sort { get; set; }
        public required string SortDirection { get; set; }
        public List<PaginationFilterHelped>? Filter { get; set; }
        public bool? ExactValues { get; set; }
        public bool? Exclude { get; set; }
        public int PagesQuantity { get; set; }
        public IEnumerable<TDocument>? Data { get; set; }
        public int TotalRows { get; set; }

    }
}
