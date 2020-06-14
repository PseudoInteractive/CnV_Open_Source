using System;
//using System.Configuration;
//using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

//using COTG.Core.Helpers;

//using Microsoft.Identity.Client;

namespace COTG.Core.Services
{
    public class IdentityService
    {
                public event EventHandler LoggedIn;
                public event EventHandler LoggedOut;
        //        // For more information about using Identity, see
        //        // https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/UWP/services/identity.md
        //        //
        //        // Read more about Microsoft Identity Client here
        //        // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki
        //        // https://docs.microsoft.com/azure/active-directory/develop/v2-overview

        //        // WTS TODO: Please create a ClientID following these steps and update the app.config IdentityClientId.
        //        // https://docs.microsoft.com/azure/active-directory/develop/quickstart-register-app
        //        private static readonly string _clientId = "c8335265-6f9e-4721-a07e-355c2cd66d6d";

        //        private static readonly string[] _graphScopes = new string[] { "openid" };

        //        private bool _integratedAuthAvailable;
        //        private IPublicClientApplication _client;
        //        public static AuthenticationResult _authenticationResult;


        //        public async Task InitializeWithAadAndPersonalMsAccounts()
        //        {
        //            _integratedAuthAvailable = false;
        //            _client = PublicClientApplicationBuilder.Create(_clientId)

        //                                                   .WithB2CAuthority("https://avaex.b2clogin.com/tfp/avaex.onmicrosoft.com/B2C_1_id")
        //                                                     .WithRedirectUri("https://avaex.b2clogin.com/oauth2/nativeclient")

        //                                                     //.WithLogging((level, message, containsPii) =>
        //                                                     //{
        //                                                     //    //Debug.WriteLine($"MSAL: {level} {message} ");
        //                                                     //}, LogLevel.Warning, enablePiiLogging: false, enableDefaultPlatformLogging: true)
        //                                                     .Build();
        //            await AcquireTokenSilentAsync();
        //        }

        //        public bool IsLoggedIn() => _authenticationResult != null;

        //        public async Task<LoginResultType> LoginAsync()
        //        {
        //            if (!NetworkInterface.GetIsNetworkAvailable())
        //            {
        //                return LoginResultType.NoNetworkAvailable;
        //            }

        //            try
        //            {
        //                var accounts = await _client.GetAccountsAsync();
        //                _authenticationResult = await _client.AcquireTokenInteractive(_graphScopes)
        //                                                     .WithAccount(accounts.FirstOrDefault())
        //                                                     .ExecuteAsync();

        //                LoggedIn?.Invoke(this, EventArgs.Empty);
        //                return LoginResultType.Success;
        //            }
        //            catch (MsalClientException ex)
        //            {
        //                if (ex.ErrorCode == "authentication_canceled")
        //                {
        //                    return LoginResultType.CancelledByUser;
        //                }

        //                return LoginResultType.UnknownError;
        //            }
        //            catch (Exception)
        //            {
        //                return LoginResultType.UnknownError;
        //            }
        //        }

        //        public bool IsAuthorized()
        //        {
        //            // TODO WTS: You can also add extra authorization checks here.
        //            // i.e.: Checks permisions of _authenticationResult.Account.Username in a database.
        //            return true;
        //        }

        //        public string GetAccountUserName()
        //        {
        //            return _authenticationResult?.Account?.Username;
        //        }

        //        public async Task LogoutAsync()
        //        {
        //            try
        //            {
        //                var accounts = await _client.GetAccountsAsync();
        //                var account = accounts.FirstOrDefault();
        //                if (account != null)
        //                {
        //                    await _client.RemoveAsync(account);
        //                }

        //                _authenticationResult = null;
        //                LoggedOut?.Invoke(this, EventArgs.Empty);
        //            }
        //            catch (MsalException)
        //            {
        //                // TODO WTS: LogoutAsync can fail please handle exceptions as appropriate to your scenario
        //                // For more info on MsalExceptions see
        //                // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/exceptions
        //            }
        //        }

        //        public async Task<string> GetAccessTokenForGraphAsync() => await GetAccessTokenAsync();

        //        private async Task<string> GetAccessTokenAsync()
        //        {
        //            var scopes = _graphScopes;
        //            var acquireTokenSuccess = await AcquireTokenSilentAsync();
        //            if (acquireTokenSuccess)
        //            {
        //                return _authenticationResult.AccessToken;
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    // Interactive authentication is required
        //                    var accounts = await _client.GetAccountsAsync();
        //                    _authenticationResult = await _client.AcquireTokenInteractive(scopes)
        //                                                         .WithAccount(accounts.FirstOrDefault())
        //                                                         .ExecuteAsync();
        //                    return _authenticationResult.AccessToken;
        //                }
        //                catch (MsalException)
        //                {
        //                    // AcquireTokenSilent and AcquireTokenInteractive failed, the session will be closed.
        //                    _authenticationResult = null;
        //                    LoggedOut?.Invoke(this, EventArgs.Empty);
        //                    return string.Empty;
        //                }
        //            }
        //        }


        //        public async Task<bool> AcquireTokenSilentAsync()
        //        {
        //            var scopes = _graphScopes;
        //            if (!NetworkInterface.GetIsNetworkAvailable())
        //            {
        //                return false;
        //            }

        //            try
        //            {
        //                var accounts = await _client.GetAccountsAsync();
        //                _authenticationResult = await _client.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
        //                                                     .ExecuteAsync();
        //                return true;
        //            }
        //            catch (MsalUiRequiredException)
        //            {
        //                if (_integratedAuthAvailable)
        //                {
        //                    try
        //                    {
        //                        _authenticationResult = await _client.AcquireTokenByIntegratedWindowsAuth(scopes)
        //                                                             .ExecuteAsync();
        //                        return true;
        //                    }
        //                    catch (MsalUiRequiredException)
        //                    {
        //                        // Interactive authentication is required
        //                        return false;
        //                    }
        //                }
        //                else
        //                {
        //                    // Interactive authentication is required
        //                    return false;
        //                }
        //            }
        //            catch (MsalException)
        //            {
        //                // TODO WTS: Silentauth failed, please handle this exception as appropriate to your scenario
        //                // For more info on MsalExceptions see
        //                // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/exceptions
        //                return false;
        //            }
        //        }
    }
}
