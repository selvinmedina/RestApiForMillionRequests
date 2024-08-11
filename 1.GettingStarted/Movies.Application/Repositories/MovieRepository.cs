using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly List<Movie> _movies = new List<Movie>();
        public Task<bool> CreateAsync(Movie movie)
        {
            _movies.Add(movie);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteByIdAsync(Guid id)
        {
            var movieIndex = _movies.FindIndex(x => x.Id == id);
            if (movieIndex == -1)
            {
                return Task.FromResult(false);
            }

            _movies.RemoveAt(movieIndex);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Movie>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Movie>>(_movies);
        }

        public Task<Movie?> GetByIdAsync(Guid id)
        {
            return Task.FromResult<Movie?>(_movies.FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> UpdateAsync(Movie movie)
        {
            var movieIndex = _movies.FindIndex(x => x.Id == movie.Id);
            if (movieIndex == -1)
            {
                return Task.FromResult(false);
            }

            _movies[movieIndex] = movie;


            return Task.FromResult(true);
        }
    }
}
