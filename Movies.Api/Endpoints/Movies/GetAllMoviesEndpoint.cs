using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies
{
    public static class GetAllMoviesEndpoint
    {
        public const string Name = "GetAllMovies";
        public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder app)
        {
            app.MapGet(ApiEndpoints.Movies.GetAll, async (
                [AsParameters] GetAllMoviesRequest request,
                IMovieService movieService,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var userId = context.GetUserId();

                var options = request.MapToOptions()
                                     .WithUserId(userId);

                var movies = await movieService.GetAllAsync(options, cancellationToken);
                var movieCount = await movieService.GetCountAsync(request.Title, request.Year, cancellationToken);

                var moviesResponse = movies.MapToResponse(
                    request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
                    request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
                    movieCount);

                return TypedResults.Ok(moviesResponse);
            })
                .Produces<MoviesResponse>(StatusCodes.Status200OK)
                .WithName($"{Name}/V1")
                .WithApiVersionSet(ApiVersioning.VersionSet)
                .HasApiVersion(1.0);

            app.MapGet(ApiEndpoints.Movies.GetAll, async (
                [AsParameters] GetAllMoviesRequest request,
                IMovieService movieService,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var userId = context.GetUserId();

                var options = request.MapToOptions()
                                     .WithUserId(userId);

                var movies = await movieService.GetAllAsync(options, cancellationToken);
                var movieCount = await movieService.GetCountAsync(request.Title, request.Year, cancellationToken);

                var moviesResponse = movies.MapToResponse(
                    request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
                    request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
                    movieCount);

                return TypedResults.Ok(moviesResponse);
            })
                .Produces<MoviesResponse>(StatusCodes.Status200OK)
                .WithName($"{Name}/V2")
                .WithApiVersionSet(ApiVersioning.VersionSet)
                .HasApiVersion(2.0);

            return app;
        }
    }
}
