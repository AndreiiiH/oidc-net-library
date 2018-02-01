using System.Threading.Tasks;

namespace AndreiiiH.OIDC.Structural.Scaffolding
{
    internal interface IUserFlows
    {
        Task RefreshToken();

        string GetLogoutUrl(string postLogoutRedirectUrl);
    }
}