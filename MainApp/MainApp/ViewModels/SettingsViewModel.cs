using System;
using System.Windows.Input;

using Xamarin.Forms;
using MainApp.AppResources;

namespace MainApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            Title = LocalizationResources.SettingsLabel;
        }
    }
}