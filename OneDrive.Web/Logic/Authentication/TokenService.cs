using Microsoft.AspNetCore.Authentication;
using OneDrive.Web.Options;

namespace OneDrive.Web.Logic.Authentication
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor context;
        private readonly MicrosoftAuthOptions microsoftAuthOptions;
        private readonly HttpClient client;

        public TokenService(IHttpContextAccessor context, IHttpClientFactory httpClientFactory, MicrosoftAuthOptions microsoftAuthOptions)
        {
            this.context = context;
            this.microsoftAuthOptions = microsoftAuthOptions;
            this.client = httpClientFactory.CreateClient();
        }

        public async Task<string> GetTokenAsync()
        {
            var utcNow = DateTime.UtcNow;
            var authenticationInfo = await context.HttpContext.AuthenticateAsync();

            var expiresAt = DateTimeOffset.Parse(authenticationInfo.Properties.GetTokenValue("expires_at")).UtcDateTime;
            if (utcNow < expiresAt)
                return authenticationInfo.Properties.GetTokenValue("access_token");

            var refreshToken = authenticationInfo.Properties.GetTokenValue("refresh_token");
            var token = await GrantRefreshedToken(refreshToken);

            authenticationInfo.Properties.UpdateTokenValue("access_token", token.AccessToken);
            authenticationInfo.Properties.UpdateTokenValue("refresh_token", token.RefreshToken);
            authenticationInfo.Properties.UpdateTokenValue("expires_at", new DateTimeOffset(utcNow.AddSeconds(token.ExpiresInSeconds)).ToString());

            await context.HttpContext.SignInAsync("Cookies", authenticationInfo.Principal, authenticationInfo.Properties);

            return token.AccessToken;
        }

        #region Private Methods

        private async Task<TokenResult> GrantRefreshedToken(string refreshToken)
        {
            const string endPoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
            var data = new[]
            {
                new KeyValuePair<string, string>("client_id", microsoftAuthOptions.ClientId),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("scope", microsoftAuthOptions.Scope),
                new KeyValuePair<string, string>("client_secret", microsoftAuthOptions.ClientSecret),
            };

            var result = await client.PostAsync(endPoint, new FormUrlEncodedContent(data));
            return await result.Content.ReadFromJsonAsync<TokenResult>(); 
        }
        #endregion
    }
}
