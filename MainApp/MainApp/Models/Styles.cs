using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MainApp.Models
{
    public static class Styles
    {
        private static Color LightRed = Color.FromHex("#ff0245");
        private static Color LightWheat = Color.FromHex("#faf0de");
        private static Color Gray = Color.FromHex("#242424");
        public static Style InvisibleButton { get; private set; }
        public static Style CommonButton { get; private set; }
        public static Color TextColor { get; private set; }
        public static Color StartPageTextColor { get; private set; }
        public static Color BackgroundColor { get; private set; }
        public static Color BarColor { get; private set; }

        public static void ChangeThemeColor()
        {
            Color commonButtonBgCol;
            if (App.Current.Properties["Theme"].ToString() == "Dark")
            {
                TextColor = LightRed;
                BackgroundColor = Gray;
                StartPageTextColor = Color.White;
                BarColor = Color.Black;
                commonButtonBgCol = Color.WhiteSmoke;
            }
            else
            {
                TextColor = Color.Black;
                BackgroundColor = LightWheat;
                StartPageTextColor = Color.Black;
                BarColor = LightRed;
                commonButtonBgCol = LightRed;
            }
            
            InvisibleButton = new Style(typeof(Button))
            {
                Setters = {
                    new Setter { Property = Button.BackgroundColorProperty, Value = BackgroundColor },
                    new Setter { Property = Button.TextColorProperty, Value = LightRed },
                    new Setter { Property = Button.CornerRadiusProperty, Value = 5},
                    new Setter { Property = Button.BorderWidthProperty, Value = 0 },
                    new Setter { Property = Button.FontSizeProperty, Value = 10 },
                    new Setter { Property = Button.PaddingProperty, Value = (0, 0, 0, 0) }
                }
            };

            CommonButton = new Style(typeof(Button))
            {
                Setters = {
                    new Setter { Property = Button.BackgroundColorProperty, Value = commonButtonBgCol },
                    new Setter { Property = Button.TextColorProperty, Value = BackgroundColor },
                    new Setter { Property = Button.BorderWidthProperty, Value = 0 },
                    new Setter { Property = Button.HorizontalOptionsProperty, Value = "FillAndExpand" },
                    new Setter { Property = Button.PaddingProperty, Value = (0, 0, 0, 0) }
                }
            };
        }
    }
}
