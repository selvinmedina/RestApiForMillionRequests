using Microsoft.AspNetCore.Mvc;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class MoviesController:ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpGet]
        [Route("movies")]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _movieRepository.GetAllAsync();
            return Ok(movies);
        }

        [HttpGet]
        [Route("movies/{id}")]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpPost]
        [Route("movies")]
        public async Task<IActionResult> CreateMovie(CreateMovieRequest request)
        {
            var movie = new Movie
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Genres = request.Genres
            };

            var result = await _movieRepository.CreateAsync(movie);
            if (!result)
            {
                return BadRequest();
            }

            var movieResponse = new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                YearOfRelease = movie.YearOfRelease,
                Genres = movie.Genres
            };

            return CreatedAtAction(nameof(GetMovieById), new { id = movieResponse.Id }, movieResponse);
        }

        [HttpPut]
        [Route("movies/{id}")]
        public async Task<IActionResult> UpdateMovie(Guid id, UpdateMovieRequest request)
        {
            var existingMovie = await _movieRepository.GetByIdAsync(id);
            if (existingMovie == null)
            {
                return NotFound();
            }

            existingMovie.Title = request.Title;
            existingMovie.YearOfRelease = request.YearOfRelease;
            //existingMovie.Genres = request.Genres;

            var result = await _movieRepository.UpdateAsync(existingMovie);
            if (!result)
            {
                return BadRequest();
            }

            return Ok(existingMovie);
        }

        [HttpDelete]
        [Route("movies/{id}")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var result = await _movieRepository.DeleteByIdAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
