using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        var command = new CommandDefinition(@"
            INSERT INTO movies (id, title, slug, year_of_release)
            VALUES (@Id, @Title, @Slug, @YearOfRelease);
        ", movie, transaction: transaction, cancellationToken: cancellationToken);

        var result = await connection.ExecuteAsync(command);

        if (result == 0)
        {
            return false;
        }

        foreach (var genre in movie.Genres)
        {
            var genreCommand = new CommandDefinition(@"
                INSERT INTO genres (id, movie_id, name)
                VALUES (@Id, @MovieId, @Name);
            ", new { Id = Guid.NewGuid(), MovieId = movie.Id, Name = genre }, transaction: transaction, cancellationToken: cancellationToken);

            await connection.ExecuteAsync(genreCommand);
        }

        transaction.Commit();

        return true;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var command = new CommandDefinition(@"
            SELECT id, title, slug, year_of_release
            FROM movies
            WHERE id = @Id;
        ", new { Id = id }, cancellationToken: cancellationToken);

        var movie = connection.QuerySingleOrDefault<Movie>(command);

        if (movie == null)
        {
            return null;
        }

        var genreCommand = new CommandDefinition(@"
            SELECT name
            FROM genres
            WHERE movie_id = @MovieId;
        ", new { MovieId = movie.Id }, cancellationToken: cancellationToken);

        var genres = connection.Query<string>(genreCommand);

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var movieDictionary = new Dictionary<Guid, Movie>();

        var command = new CommandDefinition(@"
            SELECT m.id, m.title, m.slug, m.year_of_release, g.name
            FROM movies m
            LEFT JOIN genres g
            ON m.id = g.movie_id
            ORDER BY m.title;
        ", cancellationToken: cancellationToken);

        var movies = await connection.QueryAsync<Movie, string, Movie>(command, (movie, genre) =>
        {
            if (!movieDictionary.TryGetValue(movie.Id, out var currentMovie))
            {
                currentMovie = movie;
                movieDictionary.Add(currentMovie.Id, currentMovie);
            }

            if (genre != null)
            {
                currentMovie.Genres.Add(genre);
            }

            return currentMovie;
        }, splitOn: "name");

        return movieDictionary.Values;
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        var command = new CommandDefinition(@"
            UPDATE movies
            SET title = @Title, slug = @Slug, year_of_release = @YearOfRelease
            WHERE id = @Id;
        ", movie, transaction: transaction, cancellationToken: cancellationToken);

        var result = await connection.ExecuteAsync(command);

        if (result == 0)
        {
            return false;
        }

        var deleteCommand = new CommandDefinition(@"
            DELETE FROM genres
            WHERE movie_id = @MovieId;
        ", new { MovieId = movie.Id }, transaction: transaction, cancellationToken: cancellationToken);

        await connection.ExecuteAsync(deleteCommand);

        foreach (var genre in movie.Genres)
        {
            var genreCommand = new CommandDefinition(@"
                INSERT INTO genres (id, movie_id, name)
                VALUES (@Id, @MovieId, @Name);
            ", new { Id = Guid.NewGuid(), MovieId = movie.Id, Name = genre }, transaction: transaction, cancellationToken: cancellationToken);

            await connection.ExecuteAsync(genreCommand);
        }

        transaction.Commit();

        return true;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();


        var deleteCommand = new CommandDefinition(@"
            DELETE FROM genres
            WHERE movie_id = @MovieId;
        ", new { MovieId = id }, transaction: transaction, cancellationToken: cancellationToken);

        await connection.ExecuteAsync(deleteCommand);

        var command = new CommandDefinition(@"
            DELETE FROM movies
            WHERE id = @Id;
        ", new { Id = id }, transaction: transaction, cancellationToken: cancellationToken);

        var result = await connection.ExecuteAsync(command);

        if (result == 0)
        {
            return false;
        }

        transaction.Commit();

        return true;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var command = new CommandDefinition(@"
            SELECT id, title, slug, year_of_release
            FROM movies
            WHERE slug = @Slug;
        ", new { Slug = slug }, cancellationToken: cancellationToken);

        var movie = connection.QuerySingleOrDefault<Movie>(command);

        if (movie == null)
        {
            return null;
        }

        var genreCommand = new CommandDefinition(@"
            SELECT name
            FROM genres
            WHERE movie_id = @MovieId;
        ", new { MovieId = movie.Id }, cancellationToken: cancellationToken);

        var genres = connection.Query<string>(genreCommand);

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var command = new CommandDefinition(@"
            SELECT COUNT(1)
            FROM movies
            WHERE id = @Id;
        ", new { Id = id }, cancellationToken: cancellationToken);

        var result = await connection.ExecuteScalarAsync<int>(command);

        return result > 0;
    }
}
