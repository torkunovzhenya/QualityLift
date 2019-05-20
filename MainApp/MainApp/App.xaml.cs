using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MainApp.Views;
using System.Globalization;
using MainApp.AppResources;
using MainApp.Models;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MainApp
{
    public partial class App : Application
    {

        public App()
        {
            Styles.ChangeThemeColor();

            InitializeComponent();
            
            if (!App.Current.Properties.ContainsKey("Language"))
                App.Current.Properties.Add("Language", "ru-RU");

            if (!App.Current.Properties.ContainsKey("Theme"))
                App.Current.Properties.Add("Theme", "Dark");

            if (!App.Current.Properties.ContainsKey("ImageStorage"))
                App.Current.Properties.Add("ImageStorage", "On device");

            LocalizationResources.Culture = new CultureInfo(App.Current.Properties["Language"].ToString());

            MainPage = new StartPage(this);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
