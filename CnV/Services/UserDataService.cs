﻿//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//using COTG.Core.Helpers;
//using COTG.Core.Models;
//using COTG.Core.Services;
//using COTG.Helpers;
//using COTG.Models;

//using Windows.Storage;

//namespace COTG.Services
//{
//    public class UserDataService
//    {
//        private const string _userSettingsKey = "IdentityUser";


//        static private IdentityService IdentityService => Singleton<IdentityService>.Instance;

//      //  private MicrosoftGraphService MicrosoftGraphService => Singleton<MicrosoftGraphService>.Instance;

//        public event EventHandler<UserData> UserDataUpdated;
//        //public static UserData User
//        //{
//        //    get { return _user; }
//        //    set { Set(ref _user, value); }
//        //}

//        public UserDataService()
//        {
//        }

//        //public void Initialize()
//        //{
//        //    IdentityService.LoggedIn += OnLoggedIn;
//        //    IdentityService.LoggedOut += OnLoggedOut;
//        //}

//        //public async Task<UserData> GetUserAsync()
//        //{
//        //    if (_user == null)
//        //    {
//        //        _user = await GetUserFromCacheAsync();
//        //        if (_user == null)
//        //        {
//        //            _user = GetDefaultUserData();
//        //        }
//        //    }

//        //    return _user;
//        //}

//        //private async void OnLoggedIn(object sender, EventArgs e)
//        //{
//        //    _user = await GetUserFromGraphApiAsync();
//        //    UserDataUpdated?.Invoke(this, _user);
//        //}

//        //private async void OnLoggedOut(object sender, EventArgs e)
//        //{
//        //    _user = null;
//        //    await ApplicationData.Current.LocalFolder.SaveAsync<User>(_userSettingsKey, null);
//        //}

//        //private async Task<UserData> GetUserFromCacheAsync()
//        //{
//        //    var cacheData = await ApplicationData.Current.LocalFolder.ReadAsync<User>(_userSettingsKey);
//        //    return await GetUserDataFromModel(cacheData);
//        //}

//        //private async Task<UserData> GetUserFromGraphApiAsync()
//        //{
//        //    var accessToken = await IdentityService.GetAccessTokenForGraphAsync();
//        //    if (string.IsNullOrEmpty(accessToken))
//        //    {
//        //        return null;
//        //    }

//        //    //var userData = await MicrosoftGraphService.GetUserInfoAsync(accessToken);
//        //    //if (userData != null)
//        //    //{
//        //    //    userData.Photo = await MicrosoftGraphService.GetUserPhoto(accessToken);
//        //    //    await ApplicationData.Current.LocalFolder.SaveAsync(_userSettingsKey, userData);
//        //    //}

//        //    return await GetUserDataFromModel(userData);
//        //}

//        private async Task<UserData> GetUserDataFromModel(User userData)
//        {
//            if (userData == null)
//            {
//                return null;
//            }

//            var userPhoto = string.IsNullOrEmpty(userData.Photo)
//                ? ImageHelper.ImageFromAssetsFile("DefaultIcon.png")
//                : await ImageHelper.ImageFromStringAsync(userData.Photo);

//            return new UserData()
//            {
//                Name = userData.DisplayName,
//                UserPrincipalName = userData.UserPrincipalName,
//                Photo = userPhoto
//            };
//        }

//        async public static Task<UserData> GetDefaultUserData()
//        {
//            return new UserData()
//            {
//                Name = "Hello", // IdentityService.GetAccountUserName(),
//                Photo = ImageHelper.ImageFromAssetsFile("DefaultIcon.png")
//            };
//        }
//    }
//}
