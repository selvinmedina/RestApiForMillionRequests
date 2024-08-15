using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public RatingRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
                SELECT ROUND(AVG(rating), 1) as rating
                FROM ratings
                WHERE movie_id = @MovieId
                """, new { MovieId = movieId }, cancellationToken: cancellationToken));
        }

        public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            var result = await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
                SELECT ROUND(AVG(rating), 1) as rating,
                (SELECT rating
                FROM ratings
                WHERE movie_id = @MovieId AND user_id = @UserId
                LIMIT 1)
                FROM ratings
                WHERE movie_id = @MovieId
                """, new { MovieId = movieId, UserId = userId }, cancellationToken: cancellationToken));

            return result;
        }

        public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            var result = await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO ratings (user_id, movie_id, rating)
                VALUES (@UserId, @MovieId, @Rating)
                ON CONFLICT (user_id, movie_id) DO UPDATE
                SET rating = @Rating
                """, new { UserId = userId, MovieId = movieId, Rating = rating }, cancellationToken: cancellationToken));

            return result > 0;
        }
        public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            var result = await connection.ExecuteAsync(new CommandDefinition("""
                DELETE FROM ratings
                WHERE movie_id = @MovieId AND user_id = @UserId
                """, new { MovieId = movieId, UserId = userId }, cancellationToken: cancellationToken));

            return result > 0;
        }

        public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            return await connection.QueryAsync<MovieRating>(new CommandDefinition("""
                SELECT r.rating, r.movie_id movieId, m.slug
                FROM movies m
                JOIN ratings r ON m.id = r.movie_id
                WHERE r.user_id = @UserId
                """, new { UserId = userId }, cancellationToken: cancellationToken));
        }
    }
}
