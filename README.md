# ![SimpleAuth](https://cloud.githubusercontent.com/assets/14140857/20651868/da1bd19e-b4ed-11e6-8ea4-a8065b89b576.png)
Simple OAuth2 password credentials authenticator for Xamarin Forms based on [Xamarin.Auth](https://github.com/xamarin/Xamarin.Auth/) with an customizable login dialog.

# Installation
Get it via NuGet:

``PM> Install-Package Rubito.SimpleFormsAuth``

# Usage
SimpleAuth is designed to be simple, so let's the code speak for itself:

```csharp
//set endpoint and create an new authenticator instance
var tokenEndpoint = new Uri("https://myDomain.com/Token");
var authenticator = new OAuth2PasswordCredentialsAuthenticator(tokenEndpoint);

//Use Error event to detect if the page has been aborted by the user
authenticator.Error += (obj, args) => { //handle args with care };

//listen to the Complete event for retrieving the Account object
authenticator.Complete += (obj, args) => { 
        //in this case we use Xamarin.Auth's accountstore object
        AccountStore.Create().Save(args.Account, "AwesomeApp");
    };

//get the dialog page
var page = authenticator.GetFormsUI();

//present the auth dialog page to the user the way you like
await Navigation.PushModalAsync(page);
```
And now you get this lovely dialog page:

![SimpleAuth](http://i.giphy.com/eZlSFrTg9u6Bi.gif)

Note that the dialog page should handle all error-states internally for you.

# Usage without the dialog page

This goes the same flow but you need to provide the credentials.

```csharp
//set endpoint and create an new authenticator instance
var tokenEndpoint = new Uri("https://myDomain.com/Token");
var authenticator = new OAuth2PasswordCredentialsAuthenticator(tokenEndpoint);

//we also need to set the credentials
authenticator.SetCredentials("Bill", "strongestpassword");

//before starting authentication we should add some error handling
authenticator.Error += (obj, args) => { //handle errors with care };
//listen to the complete event for retrieving the Account object
authenticator.Complete += (obj, args) => { 
        //in this case we use Xamarin.Auth's accountstore object
        AccountStore.Create().Save(args.Account, "AwesomeApp");
    };

//Start the authentication
await authenticator.SignInAsync(new CancellationToken());
```

# Account
The Account contains all relevant data to make authenticated requests.
Furthermore Xamarin.Auth provides persistance via the [Account.Store](https://github.com/xamarin/Xamarin.Auth/blob/master/GettingStarted.md) object.

# Requests
To make requests use the provided OAuth2BearerRequest object.
```csharp
//create request instance with HTTP verb, endpoint and the account object
var endpoint = new Uri("https://domain.com/api/endpoint");
var request = new OAuth2BearerRequest("GET", endpoint, null, account);

//await response
var response = await request.GetResponseAsync();
var text = response.GetResponseText();
```

# Limitations
The AuthDialogPage only supports Android and iOS in portrait mode.

For more information, please take a a look at the [Issues](https://github.com/rubit0/XamarinForms.SimpleAuth/issues) section.