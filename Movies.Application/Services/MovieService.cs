using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IValidator<Movie> _movieValidator;
        private readonly IRatingRepository _ratingRepository;
        private readonly IValidator<GetAllMoviesOptions> _getAllMoviesOptionsValidator;

        public MovieService(
            IMovieRepository movieRepository,
            IValidator<Movie> validator,
            IRatingRepository ratingRepository, IValidator<GetAllMoviesOptions> getAllMoviesOptionsValidator)
        {
            _movieRepository = movieRepository;
            _movieValidator = validator;
            _ratingRepository = ratingRepository;
            _getAllMoviesOptionsValidator = getAllMoviesOptionsValidator;
        }

        public async Task<bool> CreateAsync(Movie movie, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);

            return await _movieRepository.CreateAsync(movie, userId, cancellationToken);
        }

        public Task<bool> DeleteByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            return _movieRepository.DeleteByIdAsync(id, userId, cancellationToken);
        }

        public Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _movieRepository.ExistsByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken cancellationToken = default)
        {
            await _getAllMoviesOptionsValidator.ValidateAndThrowAsync(options, cancellationToken);

            return await _movieRepository.GetAllAsync(options, cancellationToken);
        }

        public Task<Movie?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            return _movieRepository.GetByIdAsync(id, userId, cancellationToken);
        }

        public Task<Movie?> GetBySlugAsync(string slug, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            return _movieRepository.GetBySlugAsync(slug, userId, cancellationToken);
        }

        public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken cancellationToken = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
            var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, cancellationToken);
            if (!movieExists)
            {
                return null;
            }

            await _movieRepository.UpdateAsync(movie);

            if (!userId.HasValue)
            {
                var rating = await _ratingRepository.GetRatingAsync(movie.Id, cancellationToken);
                movie.Rating = rating;

                return movie;
            }

            var userRating = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, cancellationToken);
            movie.Rating = userRating.Rating;
            movie.UserRating = userRating.UserRating;


            return movie;
        }

        public Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken cancellationToken = default)
        {
            return _movieRepository.GetCountAsync(title, yearOfRelease, cancellationToken);
        }
    }
}
