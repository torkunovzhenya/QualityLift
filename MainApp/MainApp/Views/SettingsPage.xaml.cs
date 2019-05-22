using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MainApp.AppResources;
using MainApp.Models;

namespace MainApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private RadioGroup Theme;
        private int theme = App.Current.Properties["Theme"].ToString() == "Dark" ? 0 : 1;

        private RadioGroup ImageStorage;
        private int imageStorage = App.Current.Properties["ImageStorage"].ToString() == "On device" ? 0 : 1;

        public SettingsPage()
        {
            InitializeComponent();
            
            #region ThemeRadioGroup
            Theme = new RadioGroup("Theme", theme, new RadioButton[]
            {
                new RadioButton(LocalizationResources.DarkLabel, theme == 0, 0, new Button{ Style = Styles.InvisibleButton }),
                new RadioButton(LocalizationResources.LightLabel, theme == 1, 1, new Button{ Style = Styles.InvisibleButton })
            });

            foreach (RadioButton rb in Theme.Buttons)
            {
                ThemeGrid.Children.Add(rb.ImageButton, 0, rb.Index);
                ThemeGrid.Children.Add(rb.Button, 1, rb.Index);
                rb.Button.Clicked += (s, e) => { ChangeTheme(); };
                rb.ImageButton.Clicked += (s, e) => { ChangeTheme(); };
            }
            #endregion

            #region ImageStorageRadioGroup
            ImageStorage = new RadioGroup("ImageStorage", imageStorage, new RadioButton[]
            {
                new RadioButton(LocalizationResources.OnDeviceLabel, imageStorage == 0, 0, new Button{ Style = Styles.InvisibleButton }),
                new RadioButton(LocalizationResources.OnInternetLabel, imageStorage == 1, 1, new Button{ Style = Styles.InvisibleButton })
            });

            foreach (RadioButton rb in ImageStorage.Buttons)
            {
                ImageStoreGrid.Children.Add(rb.ImageButton, 0, rb.Index);
                ImageStoreGrid.Children.Add(rb.Button, 1, rb.Index);
                rb.Button.Clicked += (s, e) => { App.Current.Properties["ImageStorage"] = ImageStorage.Active == 0 ? "On device" : "On internet"; };
                rb.ImageButton.Clicked += (s, e) => { App.Current.Properties["ImageStorage"] = ImageStorage.Active == 0 ? "On device" : "On internet"; };
            }
            #endregion

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
            Title = LocalizationResources.SettingsLabel;
            MenuPage.menuItems[1].Title = LocalizationResources.SettingsLabel;
            MenuPage.menuItems[2].Title = LocalizationResources.HelpTitle;

            LangButton.Text = LocalizationResources.ChangeLanguageButton;

            ThemeLabel.Text = LocalizationResources.ThemeLabel;
            ImageStoreLabel.Text = LocalizationResources.ImageStoreLabel;

            Theme.Buttons[0].Button.Text = LocalizationResources.DarkLabel;
            Theme.Buttons[1].Button.Text = LocalizationResources.LightLabel;
            ImageStorage.Buttons[0].Button.Text = LocalizationResources.OnDeviceLabel;
            ImageStorage.Buttons[1].Button.Text = LocalizationResources.OnInternetLabel;

            MainPage.MenuPages.Remove((int)MenuItemType.Help);
        }

        private void ChangeTheme()
        {
            string last = App.Current.Properties["Theme"].ToString();
            App.Current.Properties["Theme"] = Theme.Active == 0 ? "Dark" : "Light";

            if (last != App.Current.Properties["Theme"].ToString())
                DisplayAlert(LocalizationResources.ThemeLabel + $" - {App.Current.Properties["Theme"].ToString()}", 
                    LocalizationResources.ThemeMessage, "OK");
        }
    }
}