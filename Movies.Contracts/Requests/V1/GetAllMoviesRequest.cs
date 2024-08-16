﻿namespace Movies.Contracts.Requests.V1
{
    public class GetAllMoviesRequest : PagedRequest
    {
        public required string? Title { get; init; }
        public required int? Year { get; init; }

        public required string? SortBy { get; init; }
    }
}
