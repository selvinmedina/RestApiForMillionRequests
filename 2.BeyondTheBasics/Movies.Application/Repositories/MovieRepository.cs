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

    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        var result = await connection.ExecuteAsync(@"
            INSERT INTO movies (id, title, slug, year_of_release)
            VALUES (@Id, @Title, @Slug, @YearOfRelease);
        ", movie, transaction);

        if (result == 0)
        {
            return false;
        }

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(@"
                INSERT INTO genres (id, movie_id, name)
                VALUES (@Id, @MovieId, @Name);
            ", new { Id = Guid.NewGuid(), MovieId = movie.Id, Name = genre }, transaction);
        }

        transaction.Commit();

        return true;

    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = connection.QuerySingleOrDefault<Movie>(@"
            SELECT id, title, slug, year_of_release
            FROM movies
            WHERE id = @Id;
        ", new { Id = id });

        if (movie == null)
        {
            return null;
        }

        var genres = connection.Query<string>(@"
            SELECT name
            FROM genres
            WHERE movie_id = @MovieId;
        ", new { MovieId = movie.Id });

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var movieDictionary = new Dictionary<Guid, Movie>();

        var movies = await connection.QueryAsync<Movie, string, Movie>(@"
        SELECT m.id, m.title, m.slug, m.year_of_release, g.name
        FROM movies m
        LEFT JOIN genres g
        ON m.id = g.movie_id
        ORDER BY m.title;
    ", (movie, genre) =>
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

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        var result = await connection.ExecuteAsync(@"
            UPDATE movies
            SET title = @Title, slug = @Slug, year_of_release = @YearOfRelease
            WHERE id = @Id;
        ", movie, transaction);

        if (result == 0)
        {
            return false;
        }

        await connection.ExecuteAsync(@"
            DELETE FROM genres
            WHERE movie_id = @MovieId;
        ", new { MovieId = movie.Id }, transaction);

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(@"
                INSERT INTO genres (id, movie_id, name)
                VALUES (@Id, @MovieId, @Name);
            ", new { Id = Guid.NewGuid(), MovieId = movie.Id, Name = genre }, transaction);
        }

        transaction.Commit();

        return true;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(@"
            DELETE FROM movies
            WHERE id = @Id;
        ", new { Id = id }, transaction);

        if (result == 0)
        {
            return false;
        }

        await connection.ExecuteAsync(@"
            DELETE FROM genres
            WHERE movie_id = @MovieId;
        ", new { MovieId = id }, transaction);

        transaction.Commit();

        return true;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = connection.QuerySingleOrDefault<Movie>(@"
            SELECT id, title, slug, year_of_release
            FROM movies
            WHERE slug = @Slug;
        ", new { Slug = slug });

        if (movie == null)
        {
            return null;
        }

        var genres = connection.Query<string>(@"
            SELECT name
            FROM genres
            WHERE movie_id = @MovieId;
        ", new { MovieId = movie.Id });

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteScalarAsync<int>(@"
            SELECT COUNT(1)
            FROM movies
            WHERE id = @Id;
        ", new { Id = id });

        return result > 0;
    }
}
