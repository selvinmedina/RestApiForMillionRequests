﻿namespace Movies.Contracts.Responses
{
    public class MovieResponse
    {
        public Guid Id { get; init; }
        public required string Title { get; init; }
        public required int YearOfRelease { get; init; }
        public required IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
    }
}
