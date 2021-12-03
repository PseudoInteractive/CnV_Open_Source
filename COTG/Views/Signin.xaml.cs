using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using PlayFab;
using PlayFab.ClientModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV.Views
{
	using Game;

	public sealed partial class Signin: ContentDialog
	{
		public Signin()
		{
			this.InitializeComponent();
		}

		private async void LoginRequest(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			SettingsPage.SaveAll();
			var rv = await APlayfab.Login(SettingsPage.playerEmail,SettingsPage.playerPassword);
			if (rv == false)
			{
				args.Cancel = true;
				return;
			}
			Success();
		}

		private async void RegisterRequest(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			SettingsPage.SaveAll();

			var rv = await APlayfab.Register(SettingsPage.playerName,SettingsPage.playerEmail,SettingsPage.playerPassword);
			if (rv == false)
			{
				args.Cancel = true;
				return;
			}
			Success();
		}

		private void Success()
		{

		}


		//public string Username => SettingsPage.playerName;
		// /// <summary>
		//      /// This method is invoked when you select the Register button
		//      /// This method illustrates the flow for Registration process.
		//      /// We operate on 2 entities:
		//      /// - User Credentials of type KeyCredential
		//      /// - Public Key of type String
		//      /// We first check if user with this id already has Credentials. If so, we redirect to login procedure.
		//      /// Then we create new User Credentials. Check CreateKeyCredential for implementation details
		//      /// Then we get Base64 encoded Public Key using the new User Credentials. Check GetPublicKeyBase64 for implementation details
		//      /// Then we execute RegisterWithHello api call call. Check CallPlayFabRegisterWithHello for implementation details
		//      /// </summary>
		//      private async void RegisterRequest(object sender, RoutedEventArgs e)
		//      {
		//          // Check if the user already exists and if so log them in.
		//          KeyCredentialRetrievalResult retrieveResult = await KeyCredentialManager.OpenAsync(Username);
		//          if (retrieveResult.Status == KeyCredentialStatus.Success)
		//          {
		//              // Redirect to login procedure
		//              LogInRequest(sender, e);
		//              return;
		//          }

		//          // Create a new KeyCredential for the user on the device.
		//          var credential = await CreateKeyCredential(Username);
		//          if (credential == null) return;

		//          var publicKey = await GetPublicKeyBase64(credential);
		//          if (string.IsNullOrEmpty(publicKey)) return;
		//          // Include the name of the current device for the benefit of the user.
		//          // The server could support a Web interface that shows the user all the devices they
		//          // have signed in from and revoke access from devices they have lost.

		//          var registerResponse = await CallPlayFabRegisterWithHello(publicKey, Username);

		//          await ShowMessage("Registered and signed in with Session Ticket " + registerResponse.Result.SessionTicket);
		//      }

		//      //
		//      /// <summary>
		//      /// This method is invoked when you select the Log In button
		//      /// This method shows entities flow during the sign in process.
		//      /// We have 4 different entities:
		//      /// - User Credentials of type KeyCredential
		//      /// - Public Key Hint of type String
		//      /// - Challenge of type String
		//      /// - SignedChallenge of type String
		//      ///
		//      /// We first acquire the User Credentials. We do it based on Username. Check GetUserCredentials method for implementation details
		//      /// Next, we get Public Key Hint based on those credentials. Check GetPublicKeyHint for implementation details.
		//      /// Next we request a Challenge from PlayFab. Check GetPlayFabHelloChallenge for implementation details
		//      /// Next we sign the Challenge using User Credentials, so we obtain Signed Challenge. Check GetPlayFabHelloChallenge for implementation details
		//      /// Finally we use Signed Challenge and Public Key Hint to log into PlayFab. Check CallPlayFabLoginWithHello for implementation details
		//      /// </summary>
		//      private async void LogInRequest(object sender, RoutedEventArgs e)
		//      {
		//          // Get credentials based on current Username.
		//          var credentials = await GetUserCredentials(Username);
		//          if (credentials == null)
		//          {
		//		Assert(false);
		//           return;
		//          }

		//          // Credentials will give us Public Key. We use it to construct Public Key Hint, which is first important entity for PlayFab+UWP authentication.
		//          var publicKeyHint = GetPublicKeyHintBase64(credentials);
		//          if (string.IsNullOrEmpty(publicKeyHint))
		//          {
		//           Assert(false);
		//           return;
		//          }

		//          // Get PlayFab Challenge to sign for Windows Hello.
		//          var challenge = await GetPlayFabHelloChallenge(publicKeyHint);
		//          if (string.IsNullOrEmpty(challenge))
		//          {
		//           Assert(false);
		//           return;
		//          }

		//          // Request user to sign the challenge.
		//          var signedChallenge = await RequestUserSignChallenge(credentials, challenge);
		//          if (string.IsNullOrEmpty(signedChallenge))
		//          {
		//           Assert(false);
		//           return;
		//          }

		//          // Send the signature back to the server to confirm our identity.
		//          // The publicKeyHint tells the server which public key to use to verify the signature.
		//          var result = await CallPlayFabLoginWithHello(publicKeyHint, signedChallenge);
		//          if (result == null) return;

		//          // Report the result.
		//          await ShowMessage("Signed in with Session Ticket " + result.Result.SessionTicket);
		//      }

		//      public async Task<string> GetPublicKeyBase64(KeyCredential userCredential)
		//      {

		//          IBuffer publicKey = userCredential.RetrievePublicKey();

		//          if (publicKey == null)
		//          {
		//              await ShowMessage("Failed to get public key for credential");
		//              return null;
		//          }

		//          return CryptographicBuffer.EncodeToBase64String(publicKey);
		//      }

		//      public string GetPublicKeyHintBase64(KeyCredential userCredential)
		//      {
		//          HashAlgorithmProvider hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
		//          var publicKey = userCredential.RetrievePublicKey();
		//          IBuffer publicKeyHash = hashProvider.HashData(publicKey);
		//          return CryptographicBuffer.EncodeToBase64String(publicKeyHash);
		//      }

		//      public async Task<KeyCredential> GetUserCredentials(string userId)
		//      {
		//          // Open credential based on our Username and make sure it is successful
		//          KeyCredentialRetrievalResult retrieveResult = await KeyCredentialManager.OpenAsync(userId);

		//          if (retrieveResult.Status != KeyCredentialStatus.Success)
		//          {
		//              await ShowMessage("Error: Unable to open credentials! " + retrieveResult.Status);
		//              return null;
		//          }

		//          return retrieveResult.Credential;
		//      }

		//      public async Task<string> GetPlayFabHelloChallenge(string publicKeyHint)
		//      {
		//          // Request challenge from PlayFab and make sure response has no errors
		//          var challengeResponse = await PlayFab.PlayFabClientAPI.GetWindowsHelloChallengeAsync(new GetWindowsHelloChallengeRequest
		//          {
		//              PublicKeyHint = publicKeyHint,
		//              TitleId = PlayFab.PlayFabSettings.TitleId
		//          });

		//          if (challengeResponse.Error != null)
		//          {
		//              await ShowMessage($"Error during getting challenge: {challengeResponse.Error.Error}");
		//              return null;
		//          }

		//          return challengeResponse.Result.Challenge;

		//      }

		//      public async Task<string> RequestUserSignChallenge(KeyCredential credentials, string challenge)
		//      {
		//          IBuffer challengeBuffer = CryptographicBuffer.DecodeFromBase64String(challenge);
		//          KeyCredentialOperationResult opResult = await credentials.RequestSignAsync(challengeBuffer);

		//          if (opResult.Status != KeyCredentialStatus.Success)
		//          {
		//              await ShowMessage("Failed sign the challenge string: " + opResult.Status);
		//              return null;
		//          }

		//          return CryptographicBuffer.EncodeToBase64String(opResult.Result);
		//      }

		//      public async Task<PlayFabResult<LoginResult>> CallPlayFabLoginWithHello(string publicKeyHint, string signedChallenge)
		//      {
		//          var loginResponse = await PlayFab.PlayFabClientAPI.LoginWithWindowsHelloAsync(new LoginWithWindowsHelloRequest
		//          {
		//              ChallengeSignature = signedChallenge,
		//              PublicKeyHint = publicKeyHint
		//          });

		//          if (loginResponse.Error != null)
		//          {
		//              await ShowMessage($"Failed to log in: {loginResponse.Error.Error}");
		//              return null;
		//          }

		//          return loginResponse;
		//      }

		//      public IAsyncOperation<IUICommand> ShowMessage(string messageString)
		//      {
		//          MessageDialog message = new MessageDialog($"{messageString}");
		//          return message.ShowAsync();
		//      }

		//      public async Task<PlayFabResult<LoginResult>> CallPlayFabRegisterWithHello(string publicKey, string username)
		//      {
		//          var hostNames = NetworkInformation.GetHostNames();
		//          var localName = hostNames.FirstOrDefault(name => name.DisplayName.Contains(".local"));
		//          string computerName = localName.DisplayName.Replace(".local", "");

		//          var registerResult = await PlayFab.PlayFabClientAPI.RegisterWithWindowsHelloAsync(new RegisterWithWindowsHelloRequest
		//          {
		//              DeviceName = computerName,
		//              PublicKey = publicKey,
		//              UserName = username
		//          });

		//          if (registerResult.Error != null)
		//          {
		//              await ShowMessage(registerResult.Error.GenerateErrorReport());
		//              return null;
		//          }

		//          return registerResult;
		//      }

		//      public async Task<KeyCredential> CreateKeyCredential(string username)
		//      {
		//          KeyCredentialRetrievalResult keyCreationResult = await KeyCredentialManager.RequestCreateAsync(username, KeyCredentialCreationOption.ReplaceExisting);
		//          if (keyCreationResult.Status != KeyCredentialStatus.Success)
		//          {
		//              // User has authenticated with Windows Hello and the key credential is created.
		//              await ShowMessage("Failed to create key credential: " + keyCreationResult.Status);
		//              return null;
		//          }

		//          return keyCreationResult.Credential;
		//      }

		//  }
	}
}
