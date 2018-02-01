using System.Threading.Tasks;

namespace AndreiiiH.OIDC.Structural.Scaffolding
{
    internal interface IBaseFlow
    {
        Task GetToken(string scope);
    }
}