using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MainApp.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Android;
using Java;
using System.Diagnostics;
using MainApp.Services;
using MainApp.AppResources;
using Java.Interop;
using System.IO;
using System.Net;

namespace MainApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }
        private MediaFile photo;

        private RadioGroup Denoise;
        private int denoise = 1;

        private RadioGroup Scale;
        private int scale = 2;

        private RadioGroup Form;
        private string format = ".jpg";

        private bool Loaded = false;

        private Style InvisibleButton = new Style(typeof(Button))
        {
            Setters = {
                new Setter { Property = Button.BackgroundColorProperty, Value = Color.FromHex ("#242424") },
                new Setter { Property = Button.TextColorProperty, Value = Color.FromHex ("#ff0245") },
                new Setter { Property = Button.BorderWidthProperty, Value = 0 },
                new Setter { Property = Button.FontSizeProperty, Value = 10 },
                new Setter { Property = Button.PaddingProperty, Value = (0, 0, 0, 0) }
           }
        };

        public NewItemPage()
        {
            InitializeComponent();

            Item = new Item
            {
                Name = "Unnamed",
                Description = "No description",
                Image = new Image(),
                Preview = new Image()
            };

            Item.Image.Source = "wait.png";
            BindingContext = this;
            preview.IsVisible = false;

            #region DenoiseRadioGroup
            Denoise = new RadioGroup("Denoise", 1, new RadioButton[]
            {
                new RadioButton("0(no denoise)", false, 0, new Button{ Style = InvisibleButton }),
                new RadioButton("1(middle)", true, 1, new Button{ Style = InvisibleButton }),
                new RadioButton("2(maximum)", false, 2, new Button{ Style = InvisibleButton })
            });

            foreach (RadioButton rb in Denoise.Buttons)
            {
                DenoiseGrid.Children.Add(rb.ImageButton, 0, rb.Index);
                DenoiseGrid.Children.Add(rb.Button, 1, rb.Index);
            }
            #endregion

            #region ScaleRadioGroup
            Scale = new RadioGroup("Scale", 1, new RadioButton[]
            {
                new RadioButton("No scale", false, 0, new Button{ Style = InvisibleButton }),
                new RadioButton("2x scale", true, 1, new Button{ Style = InvisibleButton })
            });

            foreach (RadioButton rb in Scale.Buttons)
            {
                ScaleGrid.Children.Add(rb.ImageButton, 0, rb.Index);
                ScaleGrid.Children.Add(rb.Button, 1, rb.Index);
            }
            #endregion

            #region FormatRadioGroup
            Form = new RadioGroup("Format", 0, new RadioButton[]
            {
                new RadioButton(".jpeg", true, 0, new Button{ Style = InvisibleButton }),
                new RadioButton(".png", false, 1, new Button{ Style = InvisibleButton })
            });

            foreach (RadioButton rb in Form.Buttons)
            {
                FormGrid.Children.Add(rb.ImageButton, 0, rb.Index);
                FormGrid.Children.Add(rb.Button, 1, rb.Index);
            }
            #endregion
        }

        private async void ClickToLoad(object sender, EventArgs e)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            #region PermissionRequest
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                {
                    await DisplayAlert("Storage permisson", "Need a storage permission to load the image", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                status = results[Permission.Storage];
            }
            #endregion
            
            if (status == PermissionStatus.Granted)
            {
                photo = await CrossMedia.Current.PickPhotoAsync();
                if (photo == null)
                    return;

                preview.Source = ImageSource.FromFile(photo.Path);
                preview.IsVisible = true;
                Loaded = true;
            }
            else if (status != PermissionStatus.Unknown)
            {
                await DisplayAlert("Image Location Denied", "Can not continue, try again.", "OK");
            }
        }
        
        async void Save_Clicked(object sender, EventArgs e)
        {
            if (!preview.IsVisible || !Loaded)
            {
                await DisplayAlert(LocalizationResources.ErrorHeader, 
                    LocalizationResources.NotLoadImageError, "OK");
                return;
            }

            Item.Name = TextLabel.Text ?? "Unnamed";
            Item.Preview.Source = preview.Source;

            denoise = Denoise.Active;
            scale = Scale.Active + 1;
            format = Form.Active == 0 ? "jpeg" : "png";

            //#region PostRequest
            //try
            //{
            //    Item.Uri = ImageExchange.Post(denoise, scale, format, photo.GetStream());
            //}
            //catch (FormatException ex)
            //{
            //    await DisplayAlert(LocalizationResources.ErrorHeader, ex.Message, "OK");
            //    Loaded = false;
            //    return;
            //}
            //catch (Exception)
            //{
            //    await DisplayAlert(LocalizationResources.ErrorHeader,
            //        LocalizationResources.ConnectionError, "OK");
            //    Loaded = false;
            //    return;
            //}
            //#endregion

            Item.Uri = "https://waifu2x.booru.pics/outfiles/0b75c03c7840bab691a3e83c0ec1c67d4d8b20b0_s2_n1.jpg";
            Item.Id = Guid.NewGuid().ToString();
            Item.Format = format == "jpeg" ? ".jpg" : ".png";
            Item.Description = $"Time - {DateTime.Now.ToString()}, Scale = {scale}\n" +
                $"Denoise = {denoise}, Format = {Item.Format}";
            Item.Image.Source = ImageSource.FromUri(new Uri(Item.Uri));

            Loaded = true;

            AddItem(Item);

            photo.Dispose();

            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void TextLabel_Completed(object sender, EventArgs e)
        {
            Item.Name = TextLabel.Text;
        }

        private async void AddItem(Item item)
        {
            //string s = "";
            //foreach (string key in App.Current.Properties.Keys)
            //    s += "*" + key;
            //if (await DisplayAlert("Dictionary", s, "OK", "no"))
            //    return;

            if (!App.Current.Properties.ContainsKey("items"))
                App.Current.Properties.Add("items", "");

            App.Current.Properties["items"] += $"*{item.Id}";

            AddToDictionary(item);

            MessagingCenter.Send(this, "AddItem", Item);

            DisplayAlert("Dictionary", App.Current.Properties["items"].ToString(), "OK");
        }
        private void AddToDictionary(Item item)
        {
            App.Current.Properties.Add($"{item.Id}", $"*{item.Name}*{item.Description}*{item.Uri}*{item.Format}*");
            
            using (Stream file = photo.GetStream())
            {
                byte[] data = new byte[file.Length];
                file.Read(data, 0, data.Length);
                App.Current.Properties.Add($"{item.Id}data1", Convert.ToBase64String(data));
            }

            using (WebClient webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(item.Uri);
                App.Current.Properties.Add($"{item.Id}data2", Convert.ToBase64String(webClient.DownloadData(item.Uri)));
            }
        }
    }
}