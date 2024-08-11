namespace Movies.Api
{
    public static class ApiEndpoints
    {
        private const string Base = "/api";

        public static class Movies
        {
            private const string Resource = Base + "/movies";

            public const string GetAll = Resource;
            public const string Get = Resource + "/{id:guid}";
            public const string Create = Resource;
            public const string Update = Resource + "/{id:guid}";
            public const string Delete = Resource + "/{id:guid}";
        }
    }
}
