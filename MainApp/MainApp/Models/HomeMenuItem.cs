using System;
using System.Collections.Generic;
using System.Text;

namespace MainApp.Models
{
    public enum MenuItemType
    {
        QualityLift,
        Settings
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
