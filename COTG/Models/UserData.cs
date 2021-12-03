using System;

using Microsoft.UI.Xaml.Media.Imaging;

namespace CnV.Models
{
    public class UserData
    {
        public string Name { get; set; }

        public string UserPrincipalName { get; set; }

        public BitmapImage Photo { get; set; }
    }
}
