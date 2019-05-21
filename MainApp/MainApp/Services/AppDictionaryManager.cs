using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using MainApp.Models;
using Xamarin.Forms;

namespace MainApp.Services
{
    public static class AppDictionaryManager
    {
        public static void Add(Item item, Stream stream)
        {
            if (!App.Current.Properties.ContainsKey("items"))
                App.Current.Properties.Add("items", "");

            App.Current.Properties["items"] += $"*{item.Id}";
            App.Current.Properties.Add($"{item.Id}", $"*{item.Name}*{item.Description}*{item.Uri}*{item.Format}*");

            #region SavingFirstImage
            using (Stream file = stream)
            {
                byte[] data = new byte[file.Length];
                file.Read(data, 0, data.Length);
                App.Current.Properties.Add($"{item.Id}data1", Convert.ToBase64String(data));
            }
            #endregion

            #region SavingSecondImage
            try
            {
                if (App.Current.Properties["ImageStorage"].ToString() == "On device")
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] imageBytes = webClient.DownloadData(item.Uri);
                        App.Current.Properties.Add($"{item.Id}data2", Convert.ToBase64String(webClient.DownloadData(item.Uri)));
                    }
                else
                    App.Current.Properties.Add($"{item.Id}data2", "No data");
            }
            catch (WebException)
            {
                App.Current.Properties.Add($"{item.Id}data2", "No data");
            }
            #endregion
        }

        public static void Delete(string id)
        {
            App.Current.Properties["items"] = App.Current.Properties["items"].ToString().Replace($"*{id}", "");
            if (App.Current.Properties.ContainsKey(id))
            {
                App.Current.Properties.Remove(id);
                App.Current.Properties.Remove(id + "data1");
                App.Current.Properties.Remove(id + "data2");
            }
        }

        public static List<Item> GetItemsFromDictionary()
        {
            List<Item> dictItems = new List<Item>();
            string[] ids = App.Current.Properties["items"].ToString().Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string id in ids)
            {
                //Name, description, Uri and Format
                string[] elements = App.Current.Properties[id].ToString().Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

                //First Image Data
                byte[] previewimg = Convert.FromBase64String(App.Current.Properties[$"{id}data1"].ToString());

                //Second Image Data
                ImageSource source;
                string data2 = App.Current.Properties[$"{id}data2"].ToString();

                if (data2 == "No data")
                    source = ImageSource.FromUri(new Uri(elements[2]));
                else
                    source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(data2)));

                //Creating Item
                dictItems.Add(new Item
                {
                    Preview = new Image { Source = ImageSource.FromStream(() => new MemoryStream(previewimg)) },
                    Image = new Image { Source = source },
                    Id = id,
                    Name = elements[0],
                    Description = elements[1],
                    Uri = elements[2],
                    Format = elements[3]
                });
            }

            return dictItems;
        }
    }
}
