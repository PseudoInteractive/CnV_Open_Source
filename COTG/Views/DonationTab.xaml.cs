using COTG.Game;
using COTG.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace COTG.Views
{

    public sealed partial class DonationTab : UserTab, INotifyPropertyChanged
    {
        public DumbCollection<City> gridCitySource { get; } = new DumbCollection<City>();
        public static DonationTab instance;

        public DonationTab()
        {
            this.InitializeComponent();
        }
    }
}
