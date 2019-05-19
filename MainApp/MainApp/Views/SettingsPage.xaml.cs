using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MainApp.AppResources;

namespace MainApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            
            LangButton.Clicked += (s, e) =>
            {
                App.Current.Properties["Language"] = LocalizationResources.Culture.Name.Equals("en-US") ? "ru-RU" : "en-US";
                LocalizationResources.Culture = new System.Globalization.CultureInfo(App.Current.Properties["Language"].ToString());

                DisplayAlert(LocalizationResources.LanguageHeader, LocalizationResources.LanguageMessage, "OK");

                ChangeLanguage();
            };
        }

        private void ChangeLanguage()
        {
            MenuPage.menuItems[1].Title = LocalizationResources.SettingsLabel;
            LangButton.Text = LocalizationResources.ChangeLanguageButton;
        }
    }
}