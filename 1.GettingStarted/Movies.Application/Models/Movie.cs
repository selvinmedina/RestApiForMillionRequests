namespace Movies.Application.Models
{
    public class Movie
    {
        public required Guid Id { get; init; }
        public required string Title { get; set; }
        public required int YearOfRelease { get; set; }

        public required IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
    }
}
