using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MainApp.Models
{
    public enum MenuItemType
    {
        QualityLift,
        Settings,
        Help
    }
    public class HomeMenuItem : INotifyPropertyChanged
    {
        private string title;
        public MenuItemType Id { get; set; }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
