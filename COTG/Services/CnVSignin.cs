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
	using System.Globalization;
	using System.IO;
	using System.Text.Json;
	using Microsoft.Identity.Client.Extensions.Msal;
	using Microsoft.UI;
	using Microsoft.UI.Xaml;
	using Microsoft.UI.Xaml.Controls;



	internal sealed class CnVSignin
	{
		public static string?    shortName;
		public static string?    azureId;
		public static string?    email;
		public static DiscordId discordId;

		//
		// is this a security risk?
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		
		

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
		const string PolicyEditProfile   = "b2c_1_editDisplayName";
		const string PolicyResetPassword = "b2c_1_reset_password";


		/// <summary>
		/// Note: AcquireTokenInteractive will fail to get the AccessToken if "Admin Consent" has not been granted to this scope.  To achieve this:
		/// 
		/// 1st: Azure AD B2C / App registrations / [API App] / Expose an API / Add a scope
		/// 2nd: Azure AD B2C / App registrations / [This App] / API Permissions / Add a permission / My APIs / [API App] / Select & Add Permissions
		/// 3rd: Azure AD B2C / App registrations / [This App] / API Permissions / ... (next to add a permission) / Grant Admin Consent for [tenant]
		/// </summary>
		public static string[] ApiScopes = { "openid", "offline_access", "email", "profile" };//$"https://{Tenant}/CnV/demo.read" };

		/// <summary>
		/// URL for API which will receive the bearer token corresponding to this authentication
		/// </summary>
		const string ApiEndpoint = ""; //"https://jwt.ms/";

		// Shouldn't need to change these:
		const                  string AuthorityBase          = $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";
		const                  string AuthoritySignUpSignIn  = $"{AuthorityBase}{PolicySignUpSignIn}";
		const                  string AuthorityEditProfile   = $"{AuthorityBase}{PolicyEditProfile}";
//		const                  string AuthorityResetPassword = $"{AuthorityBase}{PolicyResetPassword}";
		const                  string CacheFileName          = "cnv_msal_cache.txt";
		public readonly static string CacheDir               = MsalCacheHelper.UserRootDirectory;
		static async Task  BuildPublicClientApp()
		{
			// TODO:  Mac and linux!
			var storageProperties =
					new StorageCreationPropertiesBuilder(CacheFileName,CacheDir,ClientId)
						.Build();

			PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
				.WithB2CAuthority(AuthoritySignUpSignIn)
				.WithRedirectUri(RedirectUri)
				.WithLogging(_Log, LogLevel.Verbose, true, true) // don't log P(ersonally) I(dentifiable) I(nformation) details on a regular basis
				.Build();

			var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
			cacheHelper.RegisterCache(PublicClientApp.UserTokenCache);

		//	TokenCacheHelper.Bind(PublicClientApp.UserTokenCache);
		}

		private static IPublicClientApplication PublicClientApp;

		public static async Task<bool> Go()
		{
			try
			{

			await BuildPublicClientApp();
			IEnumerable<IAccount> accounts        = null;
			try
			{
				accounts = await PublicClientApp.GetAccountsAsync(PolicySignUpSignIn);
				foreach(var a in accounts)
				{
					Debug.Log(a.Username + " " + a.Environment + " " + a.HomeAccountId);
				}

			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			IAccount? currentUserAccount = accounts?.FirstOrDefault();
			try
				{
				if (accounts != null)
				{


					if (currentUserAccount is not null)
					{
						var authResult = await PublicClientApp.AcquireTokenSilent(ApiScopes, currentUserAccount)
											.ExecuteAsync();

						ProcessUserInfo(authResult);

						return true;
					}
				}
			}
			catch(Exception ex)
			{
				Note.Show("Account not working");
			}

			

			for(int i=0;i<3;++i)
			{
				try
				{
					var authResult = await PublicClientApp.AcquireTokenInteractive(ApiScopes)
										.WithAccount(currentUserAccount)
										.WithPrompt(Prompt.SelectAccount)
										.ExecuteAsync();
					ProcessUserInfo(authResult);
					return true;
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
			return false;
		}

			return false;

		}
		//private static IAccount? GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
		//{
		//	foreach (var account in accounts)
		//	{
		//		string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
		//		if (userIdentifier.EndsWith(policy.ToLower())) return account;
		//	}

		//	return null;
		//}
		
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
		internal static async Task ClearTokenCache()
		{


			// SingOut will remove tokens from the token cache from ALL accounts, irrespective of user flow
			IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync();
			try
			{
				while(accounts.Any())
				{
					await PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
					accounts = await PublicClientApp.GetAccountsAsync();
				}

				//			playerName         = null;
			}
			catch(Exception ex)
			{
				Log($"Error signing-out user: {ex.Message}");
			}
		}

		internal static async Task SignOut()
		{
			await ClearTokenCache();

			shortName      = null;
			discordId = 0;
			azureId   = null;
			email     = null;
			await AppS.Failed("Signed out, please restart to sign in");
		}


		internal static async Task EditProfile()
		{
			try
			{
				//	IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync(PolicySignUpSignIn);
				AuthenticationResult authResult = await PublicClientApp.AcquireTokenInteractive(ApiScopes)
													//	.WithLoginHint(accounts.FirstOrDefault() )
													.WithPrompt(Prompt.NoPrompt)
														//.WithLoginHint()
													.WithB2CAuthority(AuthorityEditProfile)
													.ExecuteAsync();
				var changes  = ProcessUserInfo(authResult);
				var gp       = await PlayerGameEntity.GetAsync(Player.myId);
				if (changes.name && CnVSignin.shortName is not null)
				{
					var longName =  await PlayerTables.GetLongNameAsync(CnVSignin.shortName);
					gp.name = longName;
					Player.me.SetName(longName);
				}

				if (changes.discordId)
				{
					gp.discordId        = (long) (CnVSignin.discordId);
					Player.me.discordId = CnVSignin.discordId;
				}

				await gp.UpsertAsync();
				await APlayFab.UpdateProfileData();

				//
				//	Now update Playfab
				//
				AppS.MessageBox("Success",$"{Player.me.name}"); 
			
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}

		}

		record struct PropertyChanges
		{
			public bool azureId=false;
			public bool name=false;
			public bool email     = false;
			public bool discordId = false;
		}
		private static PropertyChanges ProcessUserInfo(AuthenticationResult authResult)
		{
			PropertyChanges result = new();
			
			if(authResult != null)
            {
				try
				{

		               using var user = ParseIdToken(authResult.IdToken);
						var      js   = user.RootElement;

					if(js.TryGetProperty("oid", out var oid))
					{
						if(azureId != oid.GetString())
						{
							if (azureId is not null)
							{
								Note.Show("Error:  Player changed identity");
								return result;
							}
							azureId        = oid.GetString();
							result.azureId = true;
						}
					}

					if(js.TryGetProperty("extension_DiscordId", out var _discordId))
						{
							var       d = _discordId.GetString();
							DiscordId v;
							//if (DiscordId.TryParse(d, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out v) || DiscordId.TryParse(d, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out v) )
							if(DiscordId.TryParse(d,NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out v))
							{
								if (discordId != v)
								{
									discordId        = v;
									result.discordId = true;
								}
							}
							else
							{
								Note.Show("Invalid DiscordID, should be number");
							}
						}
						if(js.TryGetProperty("name", out var _name))
						{
							var newName = _name.GetString();
							if (newName != shortName)
							{
								shortName   = newName;
								result.name = true;
							}
						}
						

					//  TokenInfoText.Text += $"User Identifier: {playerAzId}"                       + Environment.NewLine;
					if(TryGetEmail(js,out var _email))
					{
						if (_email is not null && _email != email)
						{
							email        = _email;
							result.email = true;
						}

					}

					//Debug.Log(TokenInfoText.Text);
					Log($"Name: {shortName}");
						Debug.Log(user.ToString());

				}
				catch(Exception e)
				{
					LogEx(e);
				}
			}

			return result;
		}

		private static bool TryGetEmail(JsonElement js,out string? email)
		{
			if ( js.TryGetProperty("emails", out var emails) )
			{
				if (emails.ValueKind == JsonValueKind.Array && emails.GetArrayLength() > 0 )
				{
					email = emails[0].GetString();
					return true;
				}
				else
				{
					Note.Show("Empty Email group");
				}

			}
			else
			{
				Note.Show("No email :(");
			}

			email = null;
			return false;
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
