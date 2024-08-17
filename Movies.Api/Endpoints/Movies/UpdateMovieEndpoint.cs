using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;

namespace Movies.Api.Endpoints.Movies
{
    public static class UpdateMovieEndpoint
    {
        public const string Name = "UPdateMovie";

        public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
        {
            app.MapPut(ApiEndpoints.Movies.Update,
                async (Guid id, UpdateMovieRequest request,
                 IMovieService movieService,
                HttpContext context,
                  IOutputCacheStore outputCacheStore,
                 LinkGenerator linkGenerator,
                 CancellationToken cancellationToken) =>
                {
                    var userId = context.GetUserId();
                    var movie = request.MapToMovie(id);
                    var updatedMovie = await movieService.UpdateAsync(movie, userId, cancellationToken);
                    if (updatedMovie is null)
                    {
                        return Results.NotFound();
                    }

                    await outputCacheStore.EvictByTagAsync("movies", cancellationToken);

                    var response = movie.MapToResponse();

                    return TypedResults.Ok(response);
                })
             .WithName(Name)
             .AllowAnonymous();

            return app;
        }
    }
}
