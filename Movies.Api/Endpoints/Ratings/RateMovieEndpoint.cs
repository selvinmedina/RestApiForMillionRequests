using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;

namespace Movies.Api.Endpoints.Ratings
{
    public static class RateMovieEndpoint
    {
        public const string Name = "RateMovie";

        public static void MapRateMovie(this IEndpointRouteBuilder app)
        {
            app.MapPut(ApiEndpoints.Movies.Rate, async (Guid movieId, RateMovieRequest request,
                IRatingService ratingService,
                IOutputCacheStore outputCacheStore,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var userId = context.GetUserId();
                var success = await ratingService.RateMovieAsync(movieId, userId!.Value, request.Rating, cancellationToken);

                return success ? Results.Ok() : Results.BadRequest();
            })
                .WithName(Name)
                .RequireAuthorization();
        }
    }
}
