using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MainApp.Models;
using MainApp.ViewModels;
using MainApp.Services;

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

        public ItemDetailPage()
        {
            InitializeComponent();

            Item = new Item
            {
                Name = "Item 1",
                Description = "This is an item description.",
                Image = new Image()
            };

            Item.Image.Source = "f0042.jpg";
            viewModel = new ItemDetailViewModel(Item);
            BindingContext = viewModel;
        }

        async void DeleteItem_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Delete Item", "Do you really want to delete this item?", "Yes", "No");

            if (!answer)
                return;

            MessagingCenter.Send(this, "RemoveItem", Item);

            string s = "";
            foreach (string key in App.Current.Properties.Keys)
                s += "*" + key;
            await DisplayAlert("Dictionary", s, "OK");
            await DisplayAlert("Dictionary", App.Current.Properties["items"].ToString(), "OK");

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
                await DisplayAlert("QualityLift", "File Saved Successfully", "Close");
            }
            else
            {
                await DisplayAlert("QualityLift", "Error while saving the file", "Close");
            }
        }
    }
}