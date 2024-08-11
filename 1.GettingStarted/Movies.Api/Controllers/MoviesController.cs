using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{
    [ApiController]
    public class MoviesController:ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpGet]
        [Route(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _movieRepository.GetAllAsync();

            var response = movies.MapToMovieResponses();

            return Ok(response);
        }

        [HttpGet]
        [Route(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var response = movie.MapToMovieResponse();

            return Ok(response);
        }

        [HttpPost]
        [Route(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> CreateMovie(CreateMovieRequest request)
        {
            var movie = request.MapToMovie();

            var result = await _movieRepository.CreateAsync(movie);
            if (!result)
            {
                return BadRequest();
            }

            var movieResponse = movie.MapToMovieResponse();

            return CreatedAtAction(nameof(GetMovieById), new { id = movieResponse.Id }, movieResponse);
        }

        [HttpPut]
        [Route(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> UpdateMovie(Guid id, UpdateMovieRequest request)
        {
            var movie = request.MapToMovie(id);

            var updated = await _movieRepository.UpdateAsync(movie);
            if (!updated)
            {
                return NotFound();
            }

            var response = movie.MapToMovieResponse();
            return Ok(response);
        }

        [HttpDelete]
        [Route(ApiEndpoints.Movies.Delete)]
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
