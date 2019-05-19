using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MainApp.Models;
using MainApp.ViewModels;
using MainApp.Services;
using MainApp.AppResources;

namespace MainApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;
        IDownloader downloader;

        public Item Item { get; set; }

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();
            
            Item = viewModel.Item;
            downloader = DependencyService.Get<IDownloader>();
            BindingContext = this.viewModel = viewModel;

            downloader.OnFileDownloaded += OnFileDownloaded;
        }

        async void DeleteItem_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert(LocalizationResources.DeleteHeader, LocalizationResources.DeleteMessage,
                LocalizationResources.YesAns, LocalizationResources.NoAns);

            if (!answer)
                return;

            MessagingCenter.Send(this, "RemoveItem", Item);

            await Navigation.PopModalAsync();
        }
        
        private void Download(object sender, EventArgs e)
        {
            downloader.DownloadFile(Item.Uri, "QualityLift Images", Item.Name + Item.Format);
        }

        private async void OnFileDownloaded(object sender, DownloadEventArgs e)
        {
            if (e.FileSaved)
            {
                await DisplayAlert("QualityLift", LocalizationResources.SuccessSaveMessage, "OK");
            }
            else
            {
                await DisplayAlert("QualityLift", LocalizationResources.FailSaveMessage, "OK");
            }
        }
    }
}