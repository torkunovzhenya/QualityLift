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
                if (LocalizationResources.Culture.Name.Equals("en-US"))
                {
                    App.Current.Properties["Language"] = "ru-RU";
                    DisplayAlert("dfsd", "dsfsdf", "fsdfs");
                }
                else
                    App.Current.Properties["Language"] = "en-US";
                LangButton.Text = LocalizationResources.ChangeLanguageButton;
            LocalizationResources.Culture = new System.Globalization.CultureInfo(App.Current.Properties["Language"].ToString());
            };
        }
    }
}