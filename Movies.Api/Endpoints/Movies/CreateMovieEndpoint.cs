using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies
{
    public static class CreateMovieEndpoint
    {
        public const string Name = "CreateMovie";

        public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder app)
        {
            app.MapPost(ApiEndpoints.Movies.Create,
                async (CreateMovieRequest request,
                 IMovieService movieService,
            HttpContext context,
                  IOutputCacheStore outputCacheStore,
                 LinkGenerator linkGenerator,
                 CancellationToken cancellationToken) =>
             {
                 var userId = context.GetUserId();

                 var movie = request.MapToMovie();

                 await movieService.CreateAsync(movie, userId, cancellationToken);
                 ;
                 await outputCacheStore.EvictByTagAsync("movies", cancellationToken);

                 var response = movie.MapToResponse();

                 return TypedResults.CreatedAtRoute(response, GetMovieEndpoint.Name, new { idOrSlug = movie.Id });
             })
             .WithName(Name)
             .Produces<MovieResponse>(StatusCodes.Status201Created)
             .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
             .RequireAuthorization(AuthConstants.TruestedMemberName);

            return app;
        }
    }
}
