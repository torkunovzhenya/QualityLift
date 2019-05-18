using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainApp.Models;
using Xamarin.Forms;
using Plugin.FilePicker;
using System.IO;

namespace MainApp.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        List<Item> items;
        public MockDataStore()
        {
            items = new List<Item>();
            List<Item> mockItems = new List<Item>();

            if (App.Current.Properties.ContainsKey("items"))
                mockItems = GetItemsFromDictionary();
            else
            {
                mockItems = new List<Item>
                {
                    new Item { Id = Guid.NewGuid().ToString(), Name = "First item", Description="This is an item description.",
                    Preview = new Image(), Image = new Image(), Uri = "",
                    Format = ".jpg"},
                    new Item { Id = Guid.NewGuid().ToString(), Name = "Second item", Description="This is an item description.",
                    Preview = new Image(), Image = new Image(), Uri = "",
                    Format = ".jpg"},
                    new Item { Id = Guid.NewGuid().ToString(), Name = "Third item", Description="This is an item description.",
                    Preview = new Image(), Image = new Image(), Uri = "",
                    Format = ".jpg"},
                };

                mockItems[0].Preview.Source = "f0012.jpg";
                mockItems[1].Preview.Source = "f0022.jpg";
                mockItems[2].Preview.Source = "f0032.jpg";
                mockItems[0].Image.Source = "f0012.jpg";
                mockItems[1].Image.Source = "f0022.jpg";
                mockItems[2].Image.Source = "f0032.jpg";
            }

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }


        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            App.Current.Properties["items"] = App.Current.Properties["items"].ToString().Replace($"*{id}", "");
            if (App.Current.Properties.ContainsKey(id))
            {
                App.Current.Properties.Remove(id);
                App.Current.Properties.Remove(id + "data1");
                App.Current.Properties.Remove(id + "data2");
            }

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        private List<Item> GetItemsFromDictionary()
        {
            List<Item> dictItems = new List<Item>();
            string[] ids = App.Current.Properties["items"].ToString().Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string id in ids)
            {
                byte[] previewimg = Convert.FromBase64String(App.Current.Properties[$"{id}data1"].ToString());
                byte[] imageimg = Convert.FromBase64String(App.Current.Properties[$"{id}data2"].ToString());

                string[] elements = App.Current.Properties[id].ToString().Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

                dictItems.Add(new Item
                {
                    Preview = new Image { Source = ImageSource.FromStream(() => new MemoryStream(previewimg)) },
                    Image = new Image { Source = ImageSource.FromStream(() => new MemoryStream(imageimg)) },
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