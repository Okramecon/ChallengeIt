using System.Text.Json.Serialization;

namespace ChallengeIt.API.Contracts.Auth
{
    public class SignInRequest
    {
        public class Login
        {
            [JsonPropertyName("login")]
            public required string UsernameOrEmail { get; set; }
            public required string Password { get; set; }
        }

        public class Refresh
        {
            public required string RefreshToken { get; set; }
        }

        public class Google
        {
            public required string IdToken { get; set; }
        }
    }
}
