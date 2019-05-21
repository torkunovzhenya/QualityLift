using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using MainApp.AppResources;

namespace MainApp.Models
{
    public class RadioButton
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public ImageButton ImageButton { get; set; }
        public Button Button { get; set; }
        public int Index { get; set; }
        public RadioButton(string name, bool status, int index, Button button)
        {
            Name = name;
            Status = status;
            Index = index;

            Button = button;
            Button.Text = name;
            Button.Clicked += (s, e) =>
            {
                Status = !Status;
            };

            ImageButton = new ImageButton();
            ImageButton.BorderWidth = 0;
            ImageButton.BackgroundColor = Styles.BackgroundColor;
            ImageButton.Clicked += (s, e) =>
            {
                Status = !Status;
            };

            ChangeActive();
        }

        public void ChangeActive()
        {
            ImageButton.Source = Status ? "Active.png" : "Non_active.png";
        }
    }
}
