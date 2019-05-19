using MainApp.Models;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MainApp.AppResources;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace MainApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        public static ObservableCollection<HomeMenuItem> menuItems = new ObservableCollection<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.QualityLift, Title="QualityLift" },
                new HomeMenuItem {Id = MenuItemType.Settings, Title=LocalizationResources.SettingsLabel }
            };
        public MenuPage()
        {
            InitializeComponent();

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };

            BindingContext = this;
        }
    }
}