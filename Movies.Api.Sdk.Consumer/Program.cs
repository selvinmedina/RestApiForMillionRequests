using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests.V1;
using Refit;
using System.Text.Json;

//var moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

var services = new ServiceCollection();

services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(s => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (requestMessage, token) => await s.GetRequiredService<AuthTokenProvider>().GetTokenAsync()
    })
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:5001"));

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

var newMovie = await moviesApi.CreateMovieAsync(new CreateMovieRequest
{
    Title = "The Matrix",
    YearOfRelease = 1999,
    Genres = new[] { "Action", "Sci-Fi" },
});

await moviesApi.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest
{
    Title = "The Matrix",
    YearOfRelease = 2000,
    Genres = new[] { "Action", "Sci-Fi", "Adventure" },
});

await moviesApi.DeleteMovieAsync(newMovie.Id);

var movie = await moviesApi.GetMovieAsync("73a77f13-1450-4baf-bbba-973b5bbe6bb5");

Console.WriteLine(JsonSerializer.Serialize(movie));

var request = new GetAllMoviesRequest
{
    Title = null,
    Year = null,
    Page = 1,
    PageSize = 10,
    SortBy = null
};

var movies = await moviesApi.GetAllMoviesAsync(request);

Console.WriteLine(JsonSerializer.Serialize(movies));

Console.ReadLine();