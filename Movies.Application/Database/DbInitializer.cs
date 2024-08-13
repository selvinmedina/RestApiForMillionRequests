using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Database
{
    public class DbInitializer
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DbInitializer(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task InitializeAsync()
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS movies
                (
                    id UUID PRIMARY KEY,
                    title TEXT NOT NULL,
                    slug TEXT NOT NULL,
                    year_of_release INT NOT NULL
                );
            ");

            // slug index
            await connection.ExecuteAsync(@"
                CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS idx_movies_slug
                ON movies
                USING btree(slug);
            ");

            // table genres
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS genres
                (
                    id UUID PRIMARY KEY,
                    movie_id UUID NOT NULL references movies (id),
                    name TEXT NOT NULL
                );
            ");

        }
    }
}
