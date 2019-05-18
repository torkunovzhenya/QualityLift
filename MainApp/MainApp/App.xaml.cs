using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MainApp.Views;
using System.Globalization;
using MainApp.AppResources;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MainApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            //CultureInfo ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            //LocalizationResources.Culture = ci;
            //DependencyService.Get<ILocalize>().SetLocale(ci);
            //App.Current.Properties.Clear();
            //App.Current.Properties.Add("items", "");
            if (App.Current.Properties.ContainsKey("language"))
                App.Current.Properties.Remove("language");
            if (!App.Current.Properties.ContainsKey("Language"))
                App.Current.Properties.Add("Language", "ru-RU");

            LocalizationResources.Culture = new CultureInfo(App.Current.Properties["Language"].ToString());

            MainPage = new StartPage(this);
            //MainPage = new MainPage();
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
