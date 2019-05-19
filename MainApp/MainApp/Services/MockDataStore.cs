using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainApp.Models;
using MainApp.Services;
using Xamarin.Forms;
using Plugin.FilePicker;
using System.IO;
using MainApp.AppResources;

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
                mockItems = AppDictionaryManager.GetItemsFromDictionary();
            else
            {
                mockItems = new List<Item>
                {
                    new Item { Id = Guid.NewGuid().ToString(), Name = "First item" + LocalizationResources.Example,
                        Preview = new Image() { Source = "f0011.jpg" }, Image = new Image() { Source = "f0012.jpg" }, Uri = "",
                        Format = ".jpg", Description="This is an item description."},
                    new Item { Id = Guid.NewGuid().ToString(), Name = "Second item" + LocalizationResources.Example,
                        Preview = new Image() { Source = "f0021.jpg" }, Image = new Image() { Source = "f0022.jpg" }, Uri = "",
                        Format = ".jpg", Description="This is an item description."},
                    new Item { Id = Guid.NewGuid().ToString(), Name = "Third item" + LocalizationResources.Example,
                        Preview = new Image() { Source = "f0031.jpg" }, Image = new Image() { Source = "f0032.jpg" }, Uri = "",
                        Format = ".jpg", Description="This is an item description."},
                };
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

            AppDictionaryManager.Delete(id);

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
    }
}