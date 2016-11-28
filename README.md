# ![SimpleAuth](https://cloud.githubusercontent.com/assets/14140857/20651868/da1bd19e-b4ed-11e6-8ea4-a8065b89b576.png)
A simple oAuth2 credentials authenticator for Xamarin Forms with an build-in login dialog.

# Usage

The smoothest way to use SimpleAuth is by just opening a new AuthDialogPage instance as a modal page and it will handle the rest for you.
Just pass the URL where the client can request an access token and a handler for the result, here a simple lambda expression is doing the job.

```csharp
BearerToken token;
var endpoint = new URI("https://domain.com/api/token");

await Navigation.PushModalAsync(new AuthDialogPage(
                                endpoint, 
                                (result) => {
                                    if(result.IsAuthenticated)
                                        token = result.Token;
                                    else
                                        //User closed dialog or some other error
                                });
```

As an third argument you can also pass an MessageHandler object for the underlying HttpClient, preferably the ModernHttpClient by Paul Betts on NuGet.

# Usage without the dialog page

* Create an authenticator object with the URL to the endpoint and the client's credentials.

```csharp
var tokenEndpoint = new URI("https://domain.com/api/token");
var authenticator = new ClientCredentialsAuthenticator(tokenEndpoint, "username", "password");
```

* Await the authentication in an async call.

```csharp
var result = await authenticator.AuthenticateAsync(_httpMessageHandler);
```

* Get the token from the results object or check failure
```csharp
if (!result.IsAuthenticated)
{
    var error = result.Message;
    //do some error handling
}

var bearerToken = result.Token;
```

# Main Guts
With SimpleAuth you only have to deal with a handful of components, let's see what we have on the plate:

## Credentials & CredentialsValidationResult
Besides holding user credentials, it can also validate its properties like for an valid email address. 

```csharp
var credentials = new Credentials("user@domain-com", "password");
var validationResult = credentials.Validate(minUsernameLength: 6, minPasswordLength: 9, isEmail: true);

if(!validationResult.isValid)
    Debug.WriteLine(validationResult.GetFormatedErrors());

>> "Invalid email address."
>> "Password must be 9 characters long."
```

## ClientCredentialsAuthenticator

We saw it's usage before but you can also pass for an Credentials object into it.
This way you can validate the Credentials before bothering the authenticator to make an authentication request to the server.


```csharp
var credentials = new Credentials("user@domain.com", "password");
var validationResult = credentials.Validate(minUsernameLength: 6, minPasswordLength: 8, isEmail: true);

if(validationResult.isValid)
{
    var authenticator = new ClientCredentialsAuthenticator(tokenEndpoint, credentials);
    //Logic
}
else
{
    //Error handling
}
```

# Limitations
Currently there is only oAuth2 based credentials authentication supported.
Also regarding the AuthDialogPage, we have only high-dpi portrait displays properly working.

For more information, please take a a look at the [Issues](https://github.com/rubit0/XamarinForms.SimpleAuth/issues) section.