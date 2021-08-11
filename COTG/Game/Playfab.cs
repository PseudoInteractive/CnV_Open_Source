using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayFab.ClientModels;
using PlayFab;
using static COTG.Debug;
/*

  New World:  
     - Add TitleId
	 - 
 */
namespace COTG.Game
{
	public class APlayfab
	{
		const int W24Id = 0x34B84;
		const string W24IdString = "34B84";
		private const string W22IdString = "2B907";
		private const string defaultIdString = "1DBCE";
		private const string W23IdString = "97E7D";

		static string titleId => JSClient.world switch { 22 => W22IdString, 23 => W23IdString, 24=> W24IdString, _ => defaultIdString };
		static PlayFabAuthenticationContext authenticationContext;
		static LoginResult login;
		public static string GetClientId()
		{
			byte[] bytes = Encoding.UTF8.GetBytes(Player.myName);
			string base64 = Convert.ToBase64String(bytes);
			// pseudo encryption
			return "!ab" + base64 + "!a";
		}
		public static string GetPlayerName()
		{
			return Player.myName;
		}
		public static void OnError(PlayFabError error)
		{
			Log($"{error.Error}, {error.ErrorMessage}");
		}

		public static async Task Login()
		{
			try
			{
				PlayFabSettings.staticSettings.TitleId = titleId;
				PlayFabSettings.GlobalErrorHandler = OnError;
				var req = new LoginWithCustomIDRequest()
				{
					CreateAccount = true,
					CustomId = GetClientId(),
					TitleId = titleId,
					InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
					{
						GetCharacterList = true,
						GetPlayerProfile = true,
						GetUserData = true,
						GetTitleData = true,
						GetUserAccountInfo = true,
						ProfileConstraints = new PlayerProfileViewConstraints()
						{
							// And make sure that both AvatarUrl and LastLogin are included.
							ShowAvatarUrl = true,
							ShowLastLogin = true,
							ShowDisplayName = true
						}

					}
				};

				var loginResult = await PlayFabClientAPI.LoginWithCustomIDAsync(req);
				if (loginResult.Error != null)
				{
					Log($"!!! Failed to log in to PlayFab: {loginResult.Error.ErrorMessage}");
					return;
				}
				login = loginResult.Result;

				authenticationContext = login.AuthenticationContext;
				if (login.InfoResultPayload.AccountInfo.Username == null)
				{

					var hr = await PlayFabClientAPI.AddUsernamePasswordAsync(new AddUsernamePasswordRequest()
					{
						AuthenticationContext = authenticationContext,
						Username = GetPlayerName(),
						Password = GetClientId(),
						Email = $"{GetPlayerName()}@conquestandvirtue.com"
					}

				);
					if (hr.Error != null)
					{
						Log($"!!! Failed AddUsernamePasswordAsync PlayFab: {hr.Error.ErrorMessage}");
						//return;
					}

				}
				if (login.InfoResultPayload.AccountInfo.TitleInfo.DisplayName == null)
				{   // Set user data
					var hr = await PlayFabClientAPI.UpdateUserTitleDisplayNameAsync(new UpdateUserTitleDisplayNameRequest()
					{
						AuthenticationContext = authenticationContext,
						DisplayName = GetPlayerName()
					});
					if (hr.Error != null)
					{
						Log($"!!! Failed to UpdateUserTitleDisplayNameAsync PlayFab: {hr.Error.ErrorMessage}");
						//return;
					}
				}
				//// wait until our alliance is fetched
				//while(!Alliance.diplomacyFetched)
				//{
				//	await Task.Delay(2500);
				//}
			
				//{ 
				//	var group = await PlayFabGroupsAPI.GetGroupAsync(new PlayFab.GroupsModels.GetGroupRequest() { AuthenticationContext = authenticationContext, GroupName = Alliance.my.name });
				//	if (group.Error != null)
				//	{
				//		Log($"!!! Failed to GetGroup PlayFab: {group.Error.ErrorMessage}");
				//		// group does not exist?
				//		var hr = await PlayFabGroupsAPI.CreateGroupAsync(new PlayFab.GroupsModels.CreateGroupRequest() { AuthenticationContext = authenticationContext, GroupName = Alliance.my.name });
				//		if (hr.Error != null)
				//		{
				//			Log($"!!! Failed to CreateGroupAsync PlayFab: {hr.Error.ErrorMessage}");

				//		}
				//		else
				//		{
				//			//	PlayFabGroupsAPI.ApplyToGroupAsync(new PlayFab.GroupsModels.ApplyToGroupRequest() {AuthenticationContext=authenticationContext,AutoAcceptOutstandingInvite=true,Group= } );
				//			//   hr.Result.
				//		}


				//	}
				//	else
				//	{
				//		var hr = await PlayFabGroupsAPI.ListMembershipAsync(new PlayFab.GroupsModels.ListMembershipRequest() { AuthenticationContext = authenticationContext, Entity = group.Result.Group });

				//	}
				//	// create a character for this world
				//	//foreach(var ch in login.InfoResultPayload.CharacterList)
				//	//{
				//	//	ch.
				//	//}

				//}
				//{
				//	var group = await PlayFabGroupsAPI.GetGroupAsync(new PlayFab.GroupsModels.GetGroupRequest() { AuthenticationContext = authenticationContext, 
				//		GroupName = $"W{JSClient.world}" });
				//	if (group.Error != null)
				//	{
				//		Log($"!!! Failed to GetGroup PlayFab: {group.Error.ErrorMessage}");
				//		// group does not exist?
				//		var hr = await PlayFabGroupsAPI.CreateGroupAsync(new PlayFab.GroupsModels.CreateGroupRequest() { AuthenticationContext = authenticationContext, GroupName = Alliance.my.name });
				//		if (hr.Error != null)
				//		{
				//			Log($"!!! Failed to CreateGroupAsync PlayFab: {hr.Error.ErrorMessage}");

				//		}
				//		else
				//		{
				//			//	PlayFabGroupsAPI.ApplyToGroupAsync(new PlayFab.GroupsModels.ApplyToGroupRequest() {AuthenticationContext=authenticationContext,AutoAcceptOutstandingInvite=true,Group= } );
				//			//   hr.Result.
				//		}


				//	}
				//	else
				//	{
				//		//var hr = await PlayFabGroupsAPI.ListGroupMembersAsync(new PlayFab.GroupsModels.ListGroupMembersRequest() { AuthenticationContext = authenticationContext, Entity = group.Result.Group });
				//		//if(hr.Error!=null)
				//		//{
				//		//	Log(hr.Error);
				//		//}
				//		//else
				//		//{
				//		//	foreach(var r in hr.Result.Members)
				//		//	{

				//		//	}
				//		//}

				//	}
				//	// create a character for this world
				//	//foreach(var ch in login.InfoResultPayload.CharacterList)
				//	//{
				//	//	ch.
				//	//}

				//}
			}
			catch (Exception _exception)
			{
				COTG.Debug.LogEx(_exception);
			}


		}
	}
}
