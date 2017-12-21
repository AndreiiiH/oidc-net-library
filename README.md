[![NuGet Pre Release](https://img.shields.io/nuget/vpre/ChaoticPixel.OIDC.svg?style=flat-square)](https://www.nuget.org/packages/ChaoticPixel.OIDC)
[![NuGet](https://img.shields.io/nuget/dt/ChaoticPixel.OIDC.svg?style=flat-square)](https://www.nuget.org/packages/ChaoticPixel.OIDC)
[![GitHub issues](https://img.shields.io/github/issues/AndreiiiH/oidc-net-library.svg?style=flat-square)](https://github.com/AndreiiiH/oidc-net-library/issues)
[![Build Status](https://travis-ci.org/AndreiiiH/oidc-net-library.svg?branch=master&style=flat-square)](https://travis-ci.org/AndreiiiH/oidc-net-library)

# Chaotic Pixel's OIDC Library for .NET v4.7.1

This is an easy-to-use library that was created as a response to the difficult implementation and security concerns of regular libraries. It implements it's own security and token caches, and even token validation, making it easy to use!

## Limitations

Currently, the library only supports the Authorization Code flow. More will be added in future builds.

## Getting the library

Installing the library is as easy as it gets! Simply open the NuGet Package Manger Console in your project, and type in the following:

### Package Manager
```
PM> Install-Package ChaoticPixel.OIDC -Version 1.2.3-alpha 
```

### .NET CLI
```
> dotnet add package ChaoticPixel.OIDC --version 1.2.3-alpha 
```

### Paket CLI
```
> paket add ChaoticPixel.OIDC --version 1.2.3-alpha 
```

# Setting up the Library

First and foremost, you have to get the Open ID Configuration from the server. This is easily done using the OpenIdConfig helper class, as such:
```csharp
OpenIdConfig config = new OpenIdConfig(clientId, clientSecret);
await config.GetConfig(configEndpointUrl);
```
__NOTE 1:__ Both `clientId` and `clientSecret` are strings.
__NOTE 2:__ `configEndpointUrl` is a string and it represents the URL the library should query for the OIDC configuration. Example: https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration

In order to use the library, each user has to have a unique TokenCache attached to them. How you store, retrieve and use the TokenCache is up to you, but this is how you implement it:

```csharp
TokenCache tokenCache = new TokenCache();
```

After that, you have to initialize a new OpenIdConfig class. This has to be unique per user as well, as it ties in with the previously created TokenCache to automatically store tokens retrieved. Once again, how you store these is up to you.

```csharp
OpenIdConnect oidc = new OpenIdConnect(config, tokenCache);
```
__NOTE:__ `config` and `tokenCache` represent the instances of `OpenIdConfig` and `TokenCache` we created earlier.

And with that, the OIDC library is ready to use!

## Authorization Code Flow

__NOTE:__ All `oidc` variables refer to the `OpenIdConnect` instance created above.

### Getting the Authorization Code

To get the authorization code we have to redirect the user to the authorization endpoint. For this, we have the `GetAuthorizationUrl` method which returns a `string`, containing the fully qualified URL of the authorization endpoint.

```csharp
string authUrl = oidc.GetAuthorizationUrl(responseType, redirectUrl, responseMode, scope, state);
```

__VARIABLES:__
1. `responseType` - What the response type should be. (Example: `"code id_token"`).
2. `redirectUrl` - Where the IDP should redirect to with the authorization code. __NOTE:__ This redirect URL has to be configured in your OIDC application.
3. `responseMode` - How the IDP should return the data to the `redirectUrl` (Example: `"form_post"`).
4. `scope` - The scope of the Authorization Code (Example: `"openid profile"`).
5. `state` - The state that should be passed to the IDP. This gets returned with the auth code for validation purposes.

__NOTE ABOUT STATE:__ You should __never__ send important information in the state of the request as it gets passed as cleartext in the URL. Instead, you can use our [State Cryptography](https://github.com/AndreiiiH/oidc-net-library#state-encryptiondecryption) helper class for security.

### Consuming the Authorization Code (and ID Token)

Once the user went through the authorization process, he will be redirected to the URL sent with the request with some data (in our example, an authorization code and an ID token). We have to consume these manually, as such:

```csharp
string authCode = formData["code"];
string idToken = formData["id_token"];

JwtSecurityToken idTokenJwt = oidc.ValidateToken(idToken);

tokenCache.SetAuthorizationCode(authCode);
tokenCache.SetIdToken(idTokenJwt);
```

As you can see, in order to get the JWT from the string, we simply validate the ID Token using the built-in method `ValidateToken`.

### Getting an Access Token

Once you have the authorization code, you can get an access token that you can then use as a Bearer authorization header for accessing resources. This is done asynchronously in the background by the library, as such:

```csharp
await oidc.GetTokens(scope, redirectUri);
```

__VARIABLES:__
1. `scope` - The scope of the Access Token. This should be no more than the scopes of the authorization code (Example: `"openid profile"`).
2. `redirectUri` - Where the tokens should be returned. This doesn't particularly matter, as we're using a POST, however, the URL used here __has__ to be registered in the OIDC application.

## Using a Refresh Token

If you add the `offline_access` scope, you will get a refresh token returned together with the access token. This can be used to get future access tokens without re-authenticating the user, and this is how:

```csharp
await oidc.RefreshToken(redirectUri);
```
__NOTE:__ `redirectUri` is a `string`.

# Other Features

Besides simplifying the use of OIDC, this library also offers a couple of helper classes to aid you in securing the flow.

## State Encryption/Decryption

One of the easiest things you can do to secure your exchange is to encrypt the state you pass to the authorization endpoint.

```csharp
string encryptedState = State.Encrypt(unencryptedState);
```
__NOTE:__ `unencryptedState` is a `string`.

And, presuming the application doesn't reset until the state is returned (as the passwords and salts used to encrypt this are generated randomly upon startup), you can easily validate it as such:

```csharp
string validatedState = State.Validate(encryptedState);
```
__NOTE:__ `encryptedState` is a `string`.

As part of the validation process, you also get returned the unencrypted state.

## RS256 Encryption/Decryption

We also offer an RS256 encryption helper class that allows you to easily encrypt/decrypt any data. It has two processes:

### Strong Encryption (randomized password and salt)

The strong encryption process uses a randomized password and salt and, as with the state, if you try to decrypt a previously encrypted string after system restart, it will fail.

```csharp
string encrypted = RS256.Encrypt(input);

string decrypted = RS256.Decrypt(encrypted);
```
__NOTE:__ `input` is a `string`.

### Simple Encryption (user-defined password)

This is a less-secure encryption as it doesn't use any random values, however, encrypted data can be decrypted at a later date as long as the password is known, regardless of system state.

```csharp
string encrypted = RS256.Encrypt(input, password);

string decrypted = RS256.Decrypt(encrypted, password);
```
__NOTE:__ Both `input` and `password` are of type `string`.