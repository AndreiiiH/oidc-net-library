namespace ChaoticPixel.OIDC.Structural.Scaffolding
{
    internal interface IAuthCodeFlow
    {
        string GetAuthorizationUrl(string scope, string state);
    }
}