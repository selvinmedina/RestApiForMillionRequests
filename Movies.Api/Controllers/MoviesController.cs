﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;


[ApiController]
[ApiVersion(1.0)]
public class MoviesController : Controller
{
    private readonly IMovieService _movieService;
    private readonly IOutputCacheStore _outputCacheStore;

    public MoviesController(IMovieService movieService, IOutputCacheStore outputCacheStore)
    {
        _movieService = movieService;
        _outputCacheStore = outputCacheStore;
    }

    [Authorize(AuthConstants.TruestedMemberName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie();
        
        await _movieService.CreateAsync(movie, userId, cancellationToken);
        
        await _outputCacheStore.EvictByTagAsync("movies", cancellationToken);
        
        var movieResponse = movie.MapToResponse();
        
        return CreatedAtAction(nameof(GetV1), new { idOrSlug = movie.Id }, movieResponse);
    }

    //[Authorize]
    //[ApiVersion(1.0, Deprecated = true)]
    [HttpGet(ApiEndpoints.Movies.Get)]
    [OutputCache]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetV1(
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
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(GetV1), values: new { idOrSlug = movie.Id })!,
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

    //[Authorize]
    [ApiVersion(2.0)]
    [HttpGet(ApiEndpoints.Movies.Get)]
    //[ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Enconding", Location = ResponseCacheLocation.Any)]
    [OutputCache]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetV2(
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
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(GetV2), values: new { idOrSlug = movie.Id })!,
            Rel = "self",
            Type = "GET"
        });

        return Ok(response);
    }

    //[Authorize]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    //[ResponseCache(
    //    Duration = 60, 
    //    VaryByQueryKeys =  new[] {"title", "year", "sortBy", "page", "pageSize"}, 
    //    VaryByHeader = "Accept, Accept-Enconding", 
    //    Location = ResponseCacheLocation.Any
    //    )]
    [OutputCache(PolicyName = "MovieCache")]
    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
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
    [HttpPut(ApiEndpoints.Movies.Update)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        await _outputCacheStore.EvictByTagAsync("movies", cancellationToken);

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var deleted = await _movieService.DeleteByIdAsync(id, userId, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        await _outputCacheStore.EvictByTagAsync("movies", cancellationToken);

        return Ok();
    }
}
