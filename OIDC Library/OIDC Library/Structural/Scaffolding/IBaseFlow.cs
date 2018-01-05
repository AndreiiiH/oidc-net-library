using System.Threading.Tasks;

namespace ChaoticPixel.OIDC.Structural.Scaffolding
{
    internal interface IBaseFlow
    {
        Task GetToken(string scope);
    }
}