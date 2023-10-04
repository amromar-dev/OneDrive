using System.Text.Json.Serialization;

namespace OneDrive.Web.Logic.Authentication
{
    public class TokenResult
    {
        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds { get; set; }
        
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }

}
