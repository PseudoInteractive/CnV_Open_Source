using COTG.Core.Models;
using COTG.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Data;

using Windows.UI.Xaml.Controls;

namespace COTG.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<CityG> cities { get; } = new ObservableCollection<CityG>();


        public MainPage()
        {
            InitializeComponent();
            Refresh();
        }
        public void Refresh()
        {
            cities.Clear();

            // TODO WTS: Replace this with your actual data
            cities.Add(new CityG() { cityId = 1010, name = "Avatar" });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
