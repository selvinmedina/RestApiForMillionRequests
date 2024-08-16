using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static MovieResponse MapToResponse(this Movie movie)
    {
        return new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Slug = movie.Slug,
            Rating = movie.Rating,
            UserRating = movie.UserRating,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres,
            Links = new()
        };
    }

    public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies, int page, int pageSize, int totalCount)
    {
        return new MoviesResponse
        {
            Items = movies.Select(MapToResponse),
            Page = page,
            PageSize = pageSize,
            Total = totalCount
        };
    }

    public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
    {
        return ratings.Select(rating => new MovieRatingResponse
        {
            MovieId = rating.MovieId,
            Rating = rating.Rating,
            Slug = rating.Slug,
        });
    }

    public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest request)
    {
        return new GetAllMoviesOptions
        {
            Title = request.Title,
            Year = request.Year,
            SortField = request.SortBy?.Trim('+', '-'),
            SortOrder = request.SortBy is null ? SortOrder.Unsorted
                        : request.SortBy?.StartsWith('-') ?? false ? SortOrder.Descending : SortOrder.Ascending,
            PageSize = request.PageSize,
            Page = request.Page
        };
    }

    public static GetAllMoviesOptions WithUserId(this GetAllMoviesOptions options, Guid? userId)
    {
        options.UserId = userId;
        return options;
    }
}
