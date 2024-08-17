using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies
{
    public static class GetMovieEndpoint
    {
        public const string Name = "GetMovie";

        public static IEndpointRouteBuilder MapGetMovie(this IEndpointRouteBuilder app)
        {

            app.MapGet(ApiEndpoints.Movies.Get,
                async (string idOrSlug,
                 IMovieService movieService,
                 HttpContext context,
                 LinkGenerator linkGenerator,
                 CancellationToken cancellationToken) =>
             {
                 var userId = context.GetUserId();

                 var movie = Guid.TryParse(idOrSlug, out var id) ?
                     await movieService.GetByIdAsync(id, userId, cancellationToken)
                     : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

                 if (movie is null)
                 {
                     return Results.NotFound();
                 }

                 var response = movie.MapToResponse();

                 var movieObj = new { id = movie.Id };

                 return TypedResults.Ok(response);
             })
             .WithName(Name)
             .Produces<MovieResponse>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound)
             .AllowAnonymous()
             .CacheOutput("MovieCache");

            return app;
        }
    }
}
