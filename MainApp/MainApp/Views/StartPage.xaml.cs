using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MainApp.AppResources;

namespace MainApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StartPage : ContentPage
	{
		public StartPage ()
		{
			InitializeComponent ();

            GreetingLabel.Text = LocalizationResources.GreetingLabel;
            StartButton.Text = LocalizationResources.GreetingButton;
            MadeBy.Text = LocalizationResources.AboutLabel;
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainPage();
        }
    }
}