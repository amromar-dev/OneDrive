namespace OneDrive.Web.Logic.Authentication
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }
}