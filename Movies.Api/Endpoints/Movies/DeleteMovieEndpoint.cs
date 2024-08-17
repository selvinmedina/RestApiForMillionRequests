using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies
{
    public static class DeleteMovieEndpoint
    {
        public const string Name = "DeleteMovie";
        public static void MapDeleteMovie(this IEndpointRouteBuilder app)
        {
            app.MapDelete(ApiEndpoints.Movies.Delete, async (Guid id,
                IMovieService movieService,
                IOutputCacheStore outputCacheStore,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var userId = context.GetUserId();
                var deleted = await movieService.DeleteByIdAsync(id, userId, cancellationToken);
                if (!deleted)
                {
                    return Results.NotFound();
                }
                await outputCacheStore.EvictByTagAsync("movies", cancellationToken);

                return Results.Ok();
            })
                .WithName(Name)
                .RequireAuthorization(AuthConstants.TruestedMemberName); ;
        }
    }
}
