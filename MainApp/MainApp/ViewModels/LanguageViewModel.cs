using System;
using System.Windows.Input;

using Xamarin.Forms;
using MainApp.AppResources;

namespace MainApp.ViewModels
{
    public class LanguageViewModel : BaseViewModel
    {
        public LanguageViewModel()
        {
            Title = LocalizationResources.SettingsLabel;
        }
    }
}