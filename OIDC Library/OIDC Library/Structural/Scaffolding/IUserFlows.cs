using System.Threading.Tasks;

namespace ChaoticPixel.OIDC.Structural.Scaffolding
{
    internal interface IUserFlows
    {
        Task RefreshToken();

        string GetLogoutUrl(string postLogoutRedirectUrl);
    }
}