using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PlayFab.ClientModels;
using PlayFab;
using PlayFab.DataModels;

/*

New World:  
- Add TitleId
- 
*/
namespace COTG.Game
{
	using System.Globalization;

	using CnVChat;
	using PlayFab.Json;
	using EntityKey = PlayFab.DataModels.EntityKey;

	public class APlayfab
	{
		private const int W0Id = 0xBE97E;
		private const string W0IdString = "BE97E";
		const int W24Id = 0x34B84;
		const int W25Id = 0x16B25;
		const string W24IdString = "34B84";
		const string W25IdString = "16B25";
		private const string W22IdString = "2B907";
		private const string defaultIdString = "1DBCE";
		private const string W23IdString = "97E7D";

		static string titleId => World.world switch
		{
			0 => W0IdString, 22 => W22IdString, 23 => W23IdString, 24 => W24IdString, 25 => W25IdString,
			_ => defaultIdString
		};

		static PlayFabAuthenticationContext authenticationContext;

		public static string GetCustomId(string playerName)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(Player.myName);
			string base64 = Convert.ToBase64String(bytes);
			// pseudo encryption
			return "!ab" + base64 + "!a";
		}

		public static void OnError(PlayFabError error)
		{
			App.Failed( error.GenerateErrorReport());
		}

		static EntityKey titlePlayerEntityKey;
		static EntityKey globalPlayerEntityKey;

		
		static async Task SetGlobalPlayerData(int playerId)
		{
			var data = new Dictionary<string, object>()
			{
				{"playerId", playerId },
			};
			var dataList = new List<SetObject>()
			{
				new SetObject()
				{
					ObjectName = "PlayerData",
					DataObject = data

				},
				// A free-tier customer may store up to 3 objects on each entity
			};


			var results = await PlayFabDataAPI.SetObjectsAsync(new SetObjectsRequest()
			{
				Entity =
					globalPlayerEntityKey, // Saved from GetEntityToken, or a specified key created from a titlePlayerId, CharacterId, etc
				Objects = dataList,
			});
			Trace(results.Result.ProfileVersion);

		}

		static async Task LoadPlayerData()
		{
			try
			{

				var getRequest = new GetObjectsRequest { Entity = globalPlayerEntityKey };
				var result = await PlayFabDataAPI.GetObjectsAsync(getRequest);
				if (result.Error is not null)
				{
					await App.Failed(result.Error.GenerateErrorReport());
				}
				else
				{
					var obs = result.Result.Objects;
					foreach (var o in obs)
					{
						Log(o.Key);
						Log(o.Value.EscapedDataObject);
						if (o.Key == "PlayerData")
						{
							var d = o.Value.DataObject as JsonObject;
							Assert(d!=null);
						
							if (d.TryGetValue("playerId", out var playerId))
							{
								Player.myId = Convert.ToInt32(playerId);
							}
						}
					}

				}
			}
			catch (Exception e)
			{
				LogEx(e);
			}
		}

		public static void Init()
		{
			PlayFabSettings.staticSettings.TitleId = titleId;
			PlayFabSettings.GlobalErrorHandler = OnError;
		}


		// Sets Authentication Context and myPlayfabId
		public static async Task<bool> Login(string email, string password)
		{
			try
			{
				Init();
				var req = new LoginWithEmailAddressRequest()
				{
					TitleId = titleId,
					Email = email,
					
					Password = password,
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
				var loginResult = await PlayFabClientAPI.LoginWithEmailAddressAsync(req);
				if (loginResult.Error is not null)
				{
					await App.Failed(loginResult.Error.GenerateErrorReport());
					return false;
				}

				var login = loginResult.Result;
				return await FinishLogin(login.AuthenticationContext,false,null);
			}
			catch (Exception e)
			{
				Debug.LogEx(e);
				return false;
			}
		}


		// Sets Authentication Context and myPlayfabId
		public static async Task<bool> Register(string playerName, string email, string password)
		{
			try
			{
				Init();
				//
				// Make sure the user does not exist
				// If we login with this id it means the user exists
				var req = new RegisterPlayFabUserRequest()
				{
					TitleId = titleId,
					Username = playerName,
					Email =  email,
					Password = password,
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
				var loginResult = await PlayFabClientAPI.RegisterPlayFabUserAsync(req);
				if (loginResult.Error is not null)
				{
					await App.Failed(loginResult.Error.GenerateErrorReport());
					return false;
				}

				var login = loginResult.Result;
				return await FinishLogin(login.AuthenticationContext, true, playerName);
			}
			catch (Exception _exception)
			{
				COTG.Debug.LogEx(_exception);
				return false;
			}
		}


//		public EntityTokenResponse EntityToken;
		public string PlayFabId;
		public string SessionTicket;
	//	public UserSettings SettingsForUser;
	//	public string Username;
		static async Task<bool> FinishLogin(PlayFabAuthenticationContext _authenticationContext,bool isNewUser, string playerName)
		{
			var PlayFabId = _authenticationContext.PlayFabId;
			Assert( isNewUser == playerName is not null);
			titlePlayerEntityKey = new () { Id = _authenticationContext.EntityId, Type =  _authenticationContext.EntityType };
			globalPlayerEntityKey = new() {Id = PlayFabId, Type = "master_player_account" };
			
			Player.myPlayfabId = ulong.Parse(PlayFabId, NumberStyles.AllowHexSpecifier);
			authenticationContext = _authenticationContext;
			var playfabId = Player.myPlayfabId.ToString("X");
			Assert( playfabId == PlayFabId);

			/// Use auth here
			await PlayerTables.LoadGamePlayers();
			
			if (isNewUser )
			{   // Set user Title Data
				var hr = await PlayFabClientAPI.UpdateUserTitleDisplayNameAsync(new UpdateUserTitleDisplayNameRequest()
				{
					AuthenticationContext = authenticationContext,
					DisplayName = playerName // Same as Title name
				});
				if (hr.Error != null)
				{
					await App.Failed(hr.Error.GenerateErrorReport());
					// return;
				}

				// new user

				
					int playerId =
						PlayerTables.playersGame.Count + 1; // Hack!  If two people do this at once it will be bad
					// Add ourselves to the list of World players for W0
					Player.myId = playerId;

					// Add ourselves to the Global list of players
					// We could reload but we are just adding it to the end of the list
					{
						var gp = new PlayerGameEntity(playerId);
						gp.playFabId = Player.myPlayfabId;
						gp.playerName = playerName;
						var t0 = PlayerGameEntity.table.UpsertAsync(gp);
						PlayerTables.playersGame[playerId] = gp;

						// Do we need to delay before refetching?
						var pw = new PlayerWorldEntity(World.world, playerId);
						var t1 = PlayerWorldEntity.table.UpsertAsync(pw);
						await Task.WhenAll(t0, t1);
					}
					SetGlobalPlayerData(playerId);
			}
			else
			{
				await LoadPlayerData();
			}

			await PlayerTables.LoadWorldPlayers();

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

			return true;
		}
	}
}
