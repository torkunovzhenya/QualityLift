using System;
using Xamarin.Forms;

namespace MainApp.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Image Image { get; set; }
        public Image Preview { get; set; }
        public string Uri { get; set; }
        public string Format { get; set; }
    }
}