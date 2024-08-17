using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;
using Refit;

namespace Movies.Api.Sdk
{
    [Headers("Authorization: Bearer")]
    public interface IMoviesApi
    {
        [Get(ApiEndpoints.Movies.Get)]
        Task<MovieResponse> GetMovieAsync(string idOrSlug);

        [Get(ApiEndpoints.Movies.GetAll)]
        Task<MoviesResponse> GetAllMoviesAsync(GetAllMoviesRequest request);

        [Post(ApiEndpoints.Movies.Create)]
        Task<MovieResponse> CreateMovieAsync([Body] CreateMovieRequest request);

        [Put(ApiEndpoints.Movies.Update)]
        Task<MovieResponse> UpdateMovieAsync(Guid id, [Body] UpdateMovieRequest request);

        [Delete(ApiEndpoints.Movies.Delete)]
        Task DeleteMovieAsync(Guid id);

        [Put(ApiEndpoints.Movies.Rate)]
        Task RateMovieAsync(Guid movieId, [Body] RateMovieRequest request);

        [Delete(ApiEndpoints.Movies.DeleteRating)]
        Task DeleteRatingAsync(Guid movieId);

        [Get(ApiEndpoints.Ratings.GetUserRatings)]
        Task<IEnumerable<MovieRatingResponse>> GetUserRatingsAsync();
    }
}
