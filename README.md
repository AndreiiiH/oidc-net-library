[![NuGet Pre Release](https://img.shields.io/nuget/vpre/ChaoticPixel.OIDC.svg?style=flat-square)](https://www.nuget.org/packages/ChaoticPixel.OIDC)
[![NuGet](https://img.shields.io/nuget/dt/ChaoticPixel.OIDC.svg?style=flat-square)](https://www.nuget.org/packages/ChaoticPixel.OIDC)
[![GitHub issues](https://img.shields.io/github/issues/AndreiiiH/oidc-net-library.svg?style=flat-square)](https://github.com/AndreiiiH/oidc-net-library/issues)
[![Build status](https://ci.appveyor.com/api/projects/status/4txwuer9348imw5f?svg=true)](https://ci.appveyor.com/project/AndreiiiH/oidc-net-library)


# Chaotic Pixel's OIDC Library for .NET v4.7.1

This is an easy-to-use library that was created as a response to the difficult implementation and security concerns of regular libraries. It implements it's own security and token caches, and even token validation, making it easy to use!

## Limitations

Currently, the library only supports the Authorization Code flow and the Client Credentials flow. More will be added in future builds.

## Getting the library

Installing the library is as easy as it gets! Simply open the NuGet Package Manger Console in your project, and type in the following:

### Package Manager
```
PM> Install-Package ChaoticPixel.OIDC -Version 1.3.4-alpha
```

### .NET CLI
```
> dotnet add package ChaoticPixel.OIDC --version 1.3.4-alpha
```

### Paket CLI
```
> paket add ChaoticPixel.OIDC --version 1.3.4-alpha
```

# Setting up the Library

First and foremost, you have to get the Open ID Configuration from the server. This is easily done using the multiple OpenIdConfig classes offered:

#### Custom Provider
```c#
OpenIdConfig config = new OpenIdConfig()
{
    ClientId = "clientId",
    ClientSecret = "clientSecret",
    RedirectUri = "https://localhost:44304/oidc/callback",
    ResponseMode = ResponseMode.FormPost
};
await config.GetConfig(configEndpointUrl);
```

#### Microsoft Online
```c#
OpenIdConfig config = new MicrosoftOnlineConfig()
{
    ClientId = "clientId",
    ClientSecret = "clientSecret",
    RedirectUri = "https://localhost:44304/oidc/callback",
    ResponseMode = ResponseMode.FormPost
};
await config.GetConfig([tenant = "common"]);
```

__NOTE 1:__ Both `clientId` and `clientSecret` are strings.
__NOTE 2:__ `configEndpointUrl` is a string and it represents the URL the library should query for the OIDC configuration. Example: https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration
__NOTE 3:__ `tenant` is the Azure Tenant you want to use. If not specified, it defaults to `common`.

In order to use the library, each user has to have a unique TokenCache attached to them. How you store, retrieve and use the TokenCache is up to you, but this is how you implement it:

```c#
TokenCache tokenCache = new TokenCache();
```

After that, you have to initialize a new OpenIdConfig class. This has to be unique per user as well, as it ties in with the previously created TokenCache to automatically store tokens retrieved. Once again, how you store these is up to you.
The OpenIdConnect class automatically detects the provider based on the type of config provided.

```c#
OpenIdConnect oidc = new OpenIdConnect(config, tokenCache);
```
__NOTE:__ `config` and `tokenCache` represent the instances of `OpenIdConfig` and `TokenCache` we created earlier.

And with that, the OIDC library is ready to use!

## Authorization Code Flow

__NOTE:__ All `oidc` variables refer to the `OpenIdConnect` instance created above.

### Getting the Authorization Code

To get the authorization code we have to redirect the user to the authorization endpoint. For this, we have the `GetAuthorizationUrl` method which returns a `string`, containing the fully qualified URL of the authorization endpoint.

```c#
string authUrl = oidc.Provider.AuthorizationCode.GetAuthorizationUrl(scope, state);
```

__VARIABLES:__
1. `scope` - The scope of the Authorization Code (Example: `"openid profile"`).
2. `state` - The state that should be passed to the IDP. This gets returned with the auth code for validation purposes.

__NOTE ABOUT STATE:__ You should __never__ send important information in the state of the request as it gets passed as cleartext in the URL. Instead, you can use our [State Cryptography](https://github.com/AndreiiiH/oidc-net-library#state-encryptiondecryption) helper class for security.

### Consuming the Authorization Code (and ID Token)

Once the user went through the authorization process, he will be redirected to the URL sent with the request with some data (in our example, an authorization code and an ID token). We have to consume these manually, as such:

```c#
string authCode = formData["code"];
string idToken = formData["id_token"];

JwtSecurityToken idTokenJwt = oidc.ValidateToken(idToken);

tokenCache.SetAuthorizationCode(authCode);
tokenCache.SetIdToken(idTokenJwt);
```

As you can see, in order to get the JWT from the string, we simply validate the ID Token using the built-in method `ValidateToken`.

### Getting an Access Token

Once you have the authorization code, you can get an access token that you can then use as a Bearer authorization header for accessing resources. This is done asynchronously in the background by the library, as such:

```c#
await oidc.Provider.AuthorizationCode.GetToken(scope);
```

__VARIABLES:__
1. `scope` - The scope of the Access Token. This should be no more than the scopes of the authorization code (Example: `"openid profile"`).

## Client Credentials Flow

Once the OIDC app has been setup correctly with all the desired information, all that's needed to get an access token is the following:

```c#
oidc.Provider.ClientCredentials.GetToken();
```

## Using a Refresh Token

If you add the `offline_access` scope, you will get a refresh token returned together with the access token. This can be used to get future access tokens without re-authenticating the user, and this is how:

```c#
await oidc.Provider.AuthorizationCode.RefreshToken();
```
__NOTE:__ `redirectUri` is a `string`.

# Other Features

Besides simplifying the use of OIDC, this library also offers a couple of helper classes to aid you in securing the flow.

## State Encryption/Decryption

One of the easiest things you can do to secure your exchange is to encrypt the state you pass to the authorization endpoint.

```c#
string encryptedState = State.Encrypt(unencryptedState);
```
__NOTE:__ `unencryptedState` is a `string`.

And, presuming the application doesn't reset until the state is returned (as the passwords and salts used to encrypt this are generated randomly upon startup), you can easily validate it as such:

```c#
string validatedState = State.Validate(encryptedState);
```
__NOTE:__ `encryptedState` is a `string`.

As part of the validation process, you also get returned the unencrypted state.

## RS256 Encryption/Decryption

We also offer an RS256 encryption helper class that allows you to easily encrypt/decrypt any data. It has two processes:

### Strong Encryption (randomized password and salt)

The strong encryption process uses a randomized password and salt and, as with the state, if you try to decrypt a previously encrypted string after system restart, it will fail.

```c#
string encrypted = RS256.Encrypt(input);

string decrypted = RS256.Decrypt(encrypted);
```
__NOTE:__ `input` is a `string`.

### Simple Encryption (user-defined password)

This is a less-secure encryption as it doesn't use any random values, however, encrypted data can be decrypted at a later date as long as the password is known, regardless of system state.

```c#
string encrypted = RS256.Encrypt(input, password);

string decrypted = RS256.Decrypt(encrypted, password);
```
__NOTE:__ Both `input` and `password` are of type `string`.