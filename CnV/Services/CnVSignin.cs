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
		const string discordIdB2C = "streetAddress";
		const string avatarUrlHashB2C = "jobTitle";
		const string discordDiscriminatorB2C = "givenName";
		const string localeB2C = "surname";


		public static string?    name;
		public static string?    azureId;
		public static string?    email;
		public static string? avatarUrlHash;
		public static string? discordDiscriminator;
		public static string? locale;
	
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
		const string ClientId ="ede9a681-b61d-4881-b264-c8dd65d5153f";// "b39598a1-4beb-4a7a-8ca5-3e06068c086b";

		/// <summary>
		/// Should be one of the choices on the Azure AD B2c / [This App] / Authentication blade
		/// </summary>
		//const string RedirectUri = $"https://{TenantName}.b2clogin.com/oauth2/nativeclient";

		//		const string RedirectUri = $"http://localhost";




		//	private static readonly string RedirectUri = "msalede9a681-b61d-4881-b264-c8dd65d5153f://auth";
		private static readonly string RedirectUri = "https://pseudoplayers.b2clogin.com/oauth2/nativeclient";
		//private static readonly string RedirectUri ="https://pseudoplayers.b2clogin.com/pseudoplayers.onmicrosoft.com/oauth2/authresp";
		//private static readonly string RedirectUri ="http://localhost";
	//			"pseudoplayers.b2clogin.com/pseudoplayers.onmicrosoft.com/oauth2/authresp";
		/// <summary>
		/// From Azure AD B2C / UserFlows blade
		/// </summary>
		const string PolicySignUpSignIn = "B2C_1A_SIGNUP_SIGNIN";//"b2c_1_signupstuff";
		const string PolicyEditProfile   = "b2c_1_editDisplayName";
		const string PolicyResetPassword = "b2c_1_passwordreset";
		/*
				https://PseudoPlayers.b2clogin.com/PseudoPlayers.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_SIGNUP_SIGNIN&client_id=b39598a1-4beb-4a7a-8ca5-3e06068c086b&nonce=defaultNonce&redirect_uri=https%3A%2F%2FPseudoPlayers.b2clogin.com%2Foauth2%2Fnativeclient&scope=openid&response_type=id_token&prompt=login

				https://pseudoplayers.b2clogin.com/tfp/pseudoplayers.onmicrosoft.com/b2c_1a_signup_signin/oauth2/v2.0/authorize?scope=openid+offline_access+profile&response_type=code&client_id=b39598a1-4beb-4a7a-8ca5-3e06068c086b&redirect_uri=https%3A%2F%2Fpseudoplayers.b2clogin.com%2Fypseudoplayers.onmicrosoft.com%2Foauth2%2Fauthresp&client-request-id=fa0fc1f6-a1f0-4481-8cb1-e5abf37d0234&x-client-SKU=MSAL.NetCore&x-client-Ver=4.40.0.0&x-client-CPU=x64&x-client-OS=Microsoft+Windows+10.0.22000&prompt=select_account&code_challenge=DOPFe5YZmVqW-d1ntCNS65ypmOJtXkYqyKZkqx-eQvA&code_challenge_method=S256&state=1000c8fe-ba17-4e62-a9e1-8769d95894e100038956-4599-44f7-9fe5-18879b312ff4&client_info=1

				https://pseudoplayers.b2clogin.com/oauth2/nativeclient?error=redirect_uri_mismatch&error_description=AADB2C90006%3a+The+redirect+URI+%27https%3a%2f%2fpseudoplayers.b2clogin.com%2fypseudoplayers.onmicrosoft.com%2foauth2%2fauthresp%27+provided+in+the+request+is+not+registered+for+the+client+id+%27b39598a1-4beb-4a7a-8ca5-3e06068c086b%27.%0d%0aCorrelation+ID%3a+48c290f8-e2d6-46c4-8bd2-371db901d80d%0d%0aTimestamp%3a+2022-01-14+11%3a57%3a30Z%0d%0a&state=1000c8fe-ba17-4e62-a9e1-8769d95894e100038956-4599-44f7-9fe5-18879b312ff4
		
		https://PseudoPlayers.b2clogin.com/PseudoPlayers.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_SIGNUP_SIGNIN&client_id=b39598a1-4beb-4a7a-8ca5-3e06068c086b&nonce=defaultNonce&redirect_uri=https%3A%2F%2FPseudoPlayers.b2clogin.com%2Foauth2%2Fnativeclient&scope=openid&response_type=id_token&prompt=login

		https://pseudoplayers.b2clogin.com/tfp/pseudoplayers.onmicrosoft.com/b2c_1a_signup_signin/oauth2/v2.0/authorize?scope=openid+offline_access+profile&response_type=code&client_id=b39598a1-4beb-4a7a-8ca5-3e06068c086b&redirect_uri=https%3A%2F%2Fpseudoplayers.b2clogin.com%2Fpseudoplayers.onmicrosoft.com%2Foauth2%2Fauthresp&client-request-id=24a95ec8-e26b-41ab-a8e7-6635e860fa20&x-client-SKU=MSAL.NetCore&x-client-Ver=4.40.0.0&x-client-CPU=x64&x-client-OS=Microsoft+Windows+10.0.22000&prompt=select_account&code_challenge=1X2Wle2SL0PPUABQLtQKlLu40X7WN821A_zrAofcKGQ&code_challenge_method=S256&state=698a6eb9-10de-4028-8dd4-ede1216ab3152653be4a-dcf6-47eb-be33-cb2ab569a6d3&client_info=1

		 */
		/// <summary>
		/// Note: AcquireTokenInteractive will fail to get the AccessToken if "Admin Consent" has not been granted to this scope.  To achieve this:
		/// 
		/// 1st: Azure AD B2C / App registrations / [API App] / Expose an API / Add a scope
		/// 2nd: Azure AD B2C / App registrations / [This App] / API Permissions / Add a permission / My APIs / [API App] / Select & Add Permissions
		/// 3rd: Azure AD B2C / App registrations / [This App] / API Permissions / ... (next to add a permission) / Grant Admin Consent for [tenant]
		/// </summary>
		public static string[] ApiScopes = { "openid", "offline_access" };// "https://PseudoPlayers.onmicrosoft.com/signin/tasks.read" };// "https://pseudoplayers.onmicrosoft.com/cnvlogin/tasks.read" };// "openid" };//$"https://{Tenant}/CnV/demo.read" };

		/// <summary>
		/// URL for API which will receive the bearer token corresponding to this authentication
		/// </summary>
		//const string ApiEndpoint = ""; //"https://jwt.ms/";

		// Shouldn't need to change these:
		const                  string AuthorityBase          = $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";
		const                  string AuthoritySignUpSignIn  = $"{AuthorityBase}{PolicySignUpSignIn}/";
		const                  string AuthorityEditProfile   = $"{AuthorityBase}{PolicyEditProfile}/";
//		const                  string AuthorityResetPassword = $"{AuthorityBase}{PolicyResetPassword}";
		const                  string CacheFileName          = "cnv_msal_cache.txt";
		public readonly static string CacheDir               = MsalCacheHelper.UserRootDirectory;

		public const string extraQueryParameters = "{domain_hint:'discord'}";

	static async Task  BuildPublicClientApp()
		{
			// TODO:  Mac and linux!
			var storageProperties =
					new StorageCreationPropertiesBuilder(CacheFileName,CacheDir)
						.Build();

			PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
				.WithB2CAuthority(AuthoritySignUpSignIn).WithExtraQueryParameters(extraQueryParameters)
				.WithRedirectUri(RedirectUri)
				.WithExperimentalFeatures(true)
				//	.WithDefaultRedirectUri()

#if SIGNINGLOG
				.WithLogging(_Log, LogLevel.Verbose, true, true) // don't log P(ersonally) I(dentifiable) I(nformation) details on a regular basis
#endif
				.Build();

			var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties,
#if SIGNINGLOG
				new("MSALCache",SourceLevels.All)
#else
				null
#endif
				);
			cacheHelper.RegisterCache(PublicClientApp.UserTokenCache);

			//CacheHelper.Bind(PublicClientApp.UserTokenCache);
		}

		private static IPublicClientApplication PublicClientApp;

		public static async Task<bool> Go()
		{
			//	await Task.Delay(500).ConfigureAwait(false);
			//			await Task.Yield();
			IEnumerable<IAccount> accounts = null;
//			IEnumerable<IAccount> accounts0 = null;
			//AppS.DispatchOnUIThread( async () => {  
			try
			{

			await BuildPublicClientApp();
				//accounts0 = (await PublicClientApp.GetAccountsAsync()).ToArray();
				//foreach(var a in accounts0)
				//{
				//	Debug.Log(a.Username + " " + a.Environment + " " + a.HomeAccountId);
				//}
				
			try
			{
				accounts = await PublicClientApp.GetAccountsAsync(PolicySignUpSignIn);
				//foreach(var a in accounts)
				//{
				//	Debug.Log(a.Username + " " + a.Environment + " " + a.HomeAccountId);
				//}

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
						var authResult = await PublicClientApp.AcquireTokenSilent(ApiScopes, currentUserAccount).WithExtraQueryParameters(extraQueryParameters)
											.ExecuteAsync();

						ProcessUserInfo(authResult);

						return true;
					}
				}
			}
			catch(Exception ex)
			{
				Note.Show("Silent didn't work, trying interactive");
			}


			return await	AppS.DispatchOnUIThreadTask(async () => { 
			for(;;)
			{
				try
				{
				var authResult = await PublicClientApp.AcquireTokenInteractive(ApiScopes)
										.WithAccount(currentUserAccount)
										//	.WithUseEmbeddedWebView(false)
										//	.WithPrompt(Prompt.SelectAccount)
										.ExecuteAsync();
					ProcessUserInfo(authResult);
					return true;
				}
				catch (Exception ex)
				{
					Log(ex.Message);
				}
				await Task.Delay(200);

			}
				});
		}
		catch(Exception e)
		{
			LogEx(e);
			return false;
		}


	//	});
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

			name      = null;
			discordId = 0;
			avatarUrlHash = null;
			azureId   = null;
			email     = null;
			await AppS.Failed("Signed out, please restart to sign in");
		}


		//internal static async Task EditProfile()
		//{
		//	try
		//	{
		//		//	IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync(PolicySignUpSignIn);
		//		AuthenticationResult authResult = await PublicClientApp.AcquireTokenInteractive(ApiScopes)
		//											//	.WithLoginHint(accounts.FirstOrDefault() )
		//											.WithPrompt(Prompt.NoPrompt)
		//												//.WithLoginHint()
		//											.WithB2CAuthority(AuthorityEditProfile)
		//											.ExecuteAsync();
		//		var changes  = ProcessUserInfo(authResult);
		//		var gp       = await PlayerGameEntity.GetAsync(Player.myId);
		//		if (changes.name && CnVSignin.name is not null)
		//		{
		//			var longName =  await PlayerTables.GetLongNameAsync(CnVSignin.name);
		//			gp.name = longName;
		//			Player.me.SetName(longName);
		//		}

		//		if (changes.discordId)
		//		{
		//			gp.discordId        = (long) (CnVSignin.discordId);
		//			Player.me.discordId = CnVSignin.discordId;
		//		}

		//		await gp.UpsertAsync();
		//		await APlayFab.UpdateProfileData(CnVSignin.email);

		//		//
		//		//	Now update Playfab
		//		//
		//		AppS.MessageBox("Success",$"{Player.me.name}"); 
			
		//	}
		//	catch(Exception ex)
		//	{
		//		LogEx(ex);
		//	}

		//}

		record struct PropertyChanges
		{
			public bool azureId;

			public bool name;
			public bool email ;
			public bool discordId;
			internal bool locale;
			internal bool avatarUrlHash;
			internal bool discordDiscriminator;
		}

		private static PropertyChanges ProcessUserInfo(AuthenticationResult authResult)
		{  
			PropertyChanges result = new();
			
			if(authResult != null)
            {
				try
				{
					azureId = authResult.UniqueId;
					using var user = ParseIdToken(authResult.IdToken);
					var js = user.RootElement;
					Log(js.ToString());
						if(js.TryGetProperty(discordIdB2C, out var _discordId))
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
					if(js.TryGetProperty(avatarUrlHashB2C, out var _avatarHash))
					{
						avatarUrlHash = _avatarHash.GetString();
						result.avatarUrlHash = true;
					}
					if(js.TryGetProperty(discordDiscriminatorB2C, out var _desc))
					{
						discordDiscriminator = _desc.GetString();
						result.discordDiscriminator = true;
					}
					if(js.TryGetProperty(localeB2C, out var _locale))
					{
						locale = _locale.GetString();
						result.locale = true;
					}
					if(js.TryGetProperty("name", out var _name))
						{
							var newName = _name.GetString();
							if (newName != name)
							{
								name   = newName;
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
					AppS.MessageBox(title:"Signed in",text:$"Welcome {Player.ToShortName(name) }.", 

						hero: "UI/menues/mentor/bgr_mentorframe_full.png");

					//Debug.Log(TokenInfoText.Text);
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
			if(js.TryGetProperty("email", out var _email))
			{
				email = _email.GetString();
				return true;
			}
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
		#if SIGNINGLOG

		private static void _Log(LogLevel level, string message, bool containsPii)
		{
			Debug.Log(message);
//			string logs = $"{level} {message}{Environment.NewLine}";
//			File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalLogs.txt", logs);
		}
#endif

	}
}
