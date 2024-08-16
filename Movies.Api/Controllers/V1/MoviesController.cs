using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers.V1;


[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [Authorize(AuthConstants.TruestedMemberName)]
    [HttpPost(ApiEndpoints.V1.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, userId, cancellationToken);
        var movieResponse = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movieResponse);
    }

    [Authorize]
    [HttpGet(ApiEndpoints.V1.Movies.Get)]
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

        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: movieObj)!,
            Rel = "update",
            Type = "PUT"
        });

        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: movieObj)!,
            Rel = "delete",
            Type = "DELETE"
        });

        return Ok(response);
    }

    [Authorize]
    [HttpGet(ApiEndpoints.V1.Movies.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var options = request.MapToOptions()
                             .WithUserId(userId);

        var movies = await _movieService.GetAllAsync(options, cancellationToken);
        var movieCount = await _movieService.GetCountAsync(request.Title, request.Year, cancellationToken);

        var moviesResponse = movies.MapToResponse(request.Page, request.PageSize, movieCount);
        return Ok(moviesResponse);
    }

    [Authorize(AuthConstants.TruestedMemberName)]
    [HttpPut(ApiEndpoints.V1.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id,
        [FromBody] UpdateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, cancellationToken);
        if (updatedMovie is null)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminPolicyName)]
    [HttpDelete(ApiEndpoints.V1.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var deleted = await _movieService.DeleteByIdAsync(id, userId, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
