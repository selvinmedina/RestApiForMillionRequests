using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings
{
    public static class GetUserRatingsEndpoint
    {
        public const string Name = "GetUserRatings";

        public static void MapGetUserRatings(this IEndpointRouteBuilder app)
        {
            app.MapGet(ApiEndpoints.Ratings.GetUserRatings, async (
                IRatingService ratingService,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var userId = context.GetUserId();
                var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, cancellationToken);

                return Results.Ok(ratings.MapToResponse());
            });
        }
    }
}
