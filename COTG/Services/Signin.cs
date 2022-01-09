// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using static CnV.Debug;
namespace CnV
{
	using System.IO;
	using System.Text.Json;
	using Microsoft.UI;
	using Microsoft.UI.Xaml;
	using Microsoft.UI.Xaml.Controls;



	internal sealed partial class Signin
	{
		public static string? name;
		public static string? azureId;
		public static string? email;


		/// <summary>
		/// B2C tenant name
		/// </summary>
		const string TenantName = "pseudoplayers";
		const string Tenant             = $"{TenantName}.onmicrosoft.com";
		const string AzureAdB2CHostname = $"{TenantName}.b2clogin.com";

		/// <summary>
		/// ClientId for the application which initiates the login functionality (this app)  
		/// </summary>
		const string ClientId = "ede9a681-b61d-4881-b264-c8dd65d5153f";

		/// <summary>
		/// Should be one of the choices on the Azure AD B2c / [This App] / Authentication blade
		/// </summary>
		const string RedirectUri = $"https://{TenantName}.b2clogin.com/oauth2/nativeclient";





		//	private static readonly string RedirectUri = "msalede9a681-b61d-4881-b264-c8dd65d5153f://auth";
		//private static readonly string RedirectUri = $"https://login.microsoftonline.com/common/oauth2/nativeclient";

		/// <summary>
		/// From Azure AD B2C / UserFlows blade
		/// </summary>
		const string PolicySignUpSignIn = "b2c_1_signupstuff";
		const string PolicyEditProfile   = "b2c_1_edit_profile";
		const string PolicyResetPassword = "b2c_1_reset";


		/// <summary>
		/// Note: AcquireTokenInteractive will fail to get the AccessToken if "Admin Consent" has not been granted to this scope.  To achieve this:
		/// 
		/// 1st: Azure AD B2C / App registrations / [API App] / Expose an API / Add a scope
		/// 2nd: Azure AD B2C / App registrations / [This App] / API Permissions / Add a permission / My APIs / [API App] / Select & Add Permissions
		/// 3rd: Azure AD B2C / App registrations / [This App] / API Permissions / ... (next to add a permission) / Grant Admin Consent for [tenant]
		/// </summary>
		public static string[] ApiScopes = { "openid", "offline_access" };//$"https://{Tenant}/CnV/demo.read" };

		/// <summary>
		/// URL for API which will receive the bearer token corresponding to this authentication
		/// </summary>
		const string ApiEndpoint = ""; //"https://jwt.ms/";

		// Shouldn't need to change these:
		const         string AuthorityBase          = $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";
		const         string AuthoritySignUpSignIn  = $"{AuthorityBase}{PolicySignUpSignIn}";
		const         string AuthorityEditProfile   = $"{AuthorityBase}{PolicyEditProfile}";
		const  string AuthorityResetPassword = $"{AuthorityBase}{PolicyResetPassword}";

		static IPublicClientApplication BuildPublicClientApp()
		{
			var PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
				.WithB2CAuthority(AuthoritySignUpSignIn)
				.WithRedirectUri(RedirectUri)
				.WithLogging(_Log, LogLevel.Verbose, true, true) // don't log P(ersonally) I(dentifiable) I(nformation) details on a regular basis
				.Build();
			TokenCacheHelper.Bind(PublicClientApp.UserTokenCache);
			return PublicClientApp;
		}


		public static async Task Go()
		{
			try
			{

			var                   PublicClientApp = BuildPublicClientApp();
			IEnumerable<IAccount> accounts        = null;
			try
			{
				accounts = await PublicClientApp.GetAccountsAsync();
				foreach(var a in accounts)
				{
					Debug.Log(a.Username + " " + a.Environment + " " + a.HomeAccountId);
				}

			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			try
			{
				if (accounts != null)
				{


					IAccount? currentUserAccount = GetAccountByPolicy(accounts, PolicySignUpSignIn);
					if (currentUserAccount is not null)
					{
						var authResult = await PublicClientApp.AcquireTokenSilent(ApiScopes, currentUserAccount)
											.ExecuteAsync();

						ProcessUserInfo(authResult);

						return;
					}
				}
			}
			catch(Exception ex)
			{
				Note.Show("Account not working");
			}

			

			for(int i=0;i<8;++i)
			{
				try
				{
					var authResult = await PublicClientApp.AcquireTokenInteractive(ApiScopes)
										.WithAccount(GetAccountByPolicy(accounts, PolicySignUpSignIn))
										.WithPrompt(Prompt.SelectAccount)
										.ExecuteAsync();
					ProcessUserInfo(authResult);
					return;
				}
				catch (Exception ex)
				{
					LogEx(ex);
				}
			}
		}
		catch(Exception e)
		{
			LogEx(e);
			throw;
		}

		}
		private static IAccount? GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
		{
			foreach (var account in accounts)
			{
				string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
				if (userIdentifier.EndsWith(policy.ToLower())) return account;
			}

			return null;
		}
		
		//private IntPtr GetHWND()
		//{
		//	return WinRT.Interop.WindowNative.GetWindowHandle(this);
		//}

		//private async void EditProfileButton_Click(object sender, RoutedEventArgs e)
  //      {
  //          var app = PublicClientApp;
  //          try
  //          {
  //              ResultText.Text = $"Calling API:{AuthorityEditProfile}";

  //              AuthenticationResult authResult = await app.AcquireTokenInteractive(ApiScopes)
  //                          .WithParentActivityOrWindow(GetHWND())
  //                          .WithB2CAuthority(AuthorityEditProfile)
  //                          .WithPrompt(Prompt.NoPrompt) 
  //                          .ExecuteAsync(new System.Threading.CancellationToken());

  //              DisplayUserInfo(authResult);
  //          }
  //          catch (Exception ex)
  //          {
  //              ResultText.Text = $"Session has expired, please sign out and back in.{AuthorityEditProfile}{Environment.NewLine}{ex}";
  //          }
  //      }

        //private async void CallApiButton_Click(object sender, RoutedEventArgs e)
        //{
        //    AuthenticationResult authResult = null;
        //    var app = PublicClientApp;
        //    var accounts = await app.GetAccountsAsync(PolicySignUpSignIn);
        //    try
        //    {
        //        authResult = await app.AcquireTokenSilent(ApiScopes, accounts.FirstOrDefault())
        //            .ExecuteAsync();
        //    }
        //    catch (MsalUiRequiredException ex)
        //    {
        //        // A MsalUiRequiredException happened on AcquireTokenSilentAsync. 
        //        // This indicates you need to call AcquireTokenAsync to acquire a token
        //        Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

        //        try
        //        {
        //            authResult = await app.AcquireTokenInteractive(ApiScopes)
        //                .WithParentActivityOrWindow(GetHWND())
        //                .ExecuteAsync();
        //        }
        //        catch (MsalException msalex)
        //        {
        //            ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{msalex}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ResultText.Text = $"Error Acquiring Token Silently:{Environment.NewLine}{ex}";
        //        return;
        //    }

        //    if (authResult != null)
        //    {
        //        if (string.IsNullOrEmpty(authResult.AccessToken))
        //        {
        //            ResultText.Text = "Access token is null (could be expired). Please do interactive log-in again." ;
        //        }
        //        else
        //        {
        //            ResultText.Text = await GetHttpContentWithToken(ApiEndpoint, authResult.AccessToken);
        //            DisplayUserInfo(authResult);
        //        }
        //    }
        //}

        /// <summary>
        /// Perform an HTTP GET request to a URL using an HTTP Authorization header
        /// </summary>
        /// <param name="url">The URL</param>
        /// <param name="token">The token</param>
        /// <returns>String containing the results of the GET operation</returns>
        static async Task<string> GetHttpContentWithToken(string url, string token)
        {
            try
            {
				using var           httpClient = new HttpClient();

				using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        internal static async Task SignOut()
        {
			var PublicClientApp = BuildPublicClientApp();

			// SingOut will remove tokens from the token cache from ALL accounts, irrespective of user flow
			IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync();
            try
            {
                while (accounts.Any())
                {
                    await PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await PublicClientApp.GetAccountsAsync();
                }

	//			playerName         = null;
				await AppS.Failed("Please restart");
            }
            catch (Exception ex)
            {
                Log( $"Error signing-out user: {ex.Message}");
            }
        }


        private static void ProcessUserInfo(AuthenticationResult authResult)
        {
            if (authResult != null)
            {
				try
				{

		               using var user = ParseIdToken(authResult.IdToken);
						var      js   = user.RootElement;
						name         =  js.GetAsString("name");
						azureId         =  js.GetAsString("oid");
					
		                Log( $"Name: {name}"    );
		              //  TokenInfoText.Text += $"User Identifier: {playerAzId}"                       + Environment.NewLine;

						if ( js.TryGetProperty("emails", out var emails) )
						{

							foreach (var v in emails.EnumerateArray())
							{
								if (email == null)
									email = v.GetString();
							}
						}
						else
						{
							Note.Show("No email :(");
						}

						//Debug.Log(TokenInfoText.Text);
						Debug.Log(user.ToString());

				}
				catch(Exception e)
				{
					LogEx(e);
				}
			}
		}

		static JsonDocument ParseIdToken(string idToken)
        {
            // Parse the idToken to get user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
			Debug.Log(idToken);
			return JsonDocument.Parse(idToken);
        }

        private static string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }
		private static void _Log(LogLevel level, string message, bool containsPii)
		{
			Debug.Log(message);
//			string logs = $"{level} {message}{Environment.NewLine}";
//			File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalLogs.txt", logs);
		}

	}
}
