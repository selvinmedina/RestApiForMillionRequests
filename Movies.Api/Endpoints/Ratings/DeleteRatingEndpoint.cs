using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings
{
    public static class DeleteRatingEndpoint
    {
        public const string Name = "DeleteRating";

        public static void MapDeleteRating(this IEndpointRouteBuilder app)
        {
            app.MapDelete(ApiEndpoints.Movies.DeleteRating, async (
                Guid movieId,
                IRatingService ratingService,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var userId = context.GetUserId();
                var success = await ratingService.DeleteRatingAsync(movieId, userId!.Value, cancellationToken);

                return success ? Results.Ok() : Results.NotFound();
            })
                .WithName(Name)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }
}
