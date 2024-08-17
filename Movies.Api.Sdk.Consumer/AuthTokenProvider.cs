using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace Movies.Api.Sdk.Consumer
{
    public class AuthTokenProvider
    {
        private readonly HttpClient _httpClient;
        private string _cachedToken = string.Empty;
        private static readonly SemaphoreSlim Lock = new(1, 1);

        public AuthTokenProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrWhiteSpace(_cachedToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(_cachedToken);

                var expiryTimeText = jwt.Claims.Single(c => c.Type == "exp").Value;

                var expiryTime = UnixTimeStampToDateTime(int.Parse(expiryTimeText));

                if (expiryTime > DateTime.UtcNow.AddMinutes(5))
                {
                    return _cachedToken;

                }
            }

            await Lock.WaitAsync();
            try
            {
                if (!string.IsNullOrWhiteSpace(_cachedToken))
                {
                    return _cachedToken;
                }

                var response = await _httpClient.PostAsJsonAsync("https://localhost:5003/token", new
                {
                    userid = "d8566de3-b1a6-4a9b-b842-8e3887a82e41",
                    email = "selvin@medina.com",
                    customClaims = new Dictionary<string, string>
                    {
                        { "admin", "true" },
                        { "trusted_member", "true" }
                    }
                });

                response.EnsureSuccessStatusCode();

                var token = await response.Content.ReadAsStringAsync();

                _cachedToken = token;

                return token;
            }
            finally
            {
                Lock.Release();
            }
        }

        private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime;
        }

    }
}
