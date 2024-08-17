﻿using Movies.Api.Endpoints.Movies;
using Movies.Api.Endpoints.Ratings;

namespace Movies.Api.Endpoints
{
    public static class EndpointsExtensions
    {
        public static IEndpointRouteBuilder MapApiEndpints(this IEndpointRouteBuilder app)
        {
            app.MapMovieEndpoints();
            app.MapRatingEndpoints();
            return app;
        }
    }
}
