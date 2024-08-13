using Movies.Application.Models;

namespace Movies.Application.Services
{
    public interface IMovieService
    {
        Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default);

        Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

        Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Movie?> UpdateAsync(Movie movie, CancellationToken cancellationToken = default);

        Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
