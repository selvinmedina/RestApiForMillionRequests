using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers.V2;


[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [Authorize]
    [HttpGet(ApiEndpoints.V2.Movies.Get)]
    public async Task<IActionResult> Get(
        [FromRoute] string idOrSlug,
        [FromServices] LinkGenerator linkGenerator,
        CancellationToken cancellationToken
        )
    {
        var userId = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id) ?
            await _movieService.GetByIdAsync(id, userId, cancellationToken)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();

        var movieObj = new { id = movie.Id };

        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new { idOrSlug = movie.Id })!,
            Rel = "self",
            Type = "GET"
        });

        return Ok(response);
    }
}
