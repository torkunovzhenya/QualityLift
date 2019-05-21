using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MainApp.Models
{
    public class RadioGroup
    {
        public List<RadioButton> Buttons { get; set; }
        public Label Label { get; set; }
        public int Active { get; private set; }

        public RadioGroup(string label, int active, RadioButton[] buttons)
        {
            Active = active;
            Buttons = new List<RadioButton>(buttons.Length);
            Label = new Label { Text = label };
            
            foreach (RadioButton button in buttons)
            {
                Buttons.Add(button);
                button.Button.Clicked += OnChange;
                button.ImageButton.Clicked += OnChange;
            }
        }

        private void OnChange(object o, EventArgs e)
        {
            for (int i = 0; i < Buttons.Count; i++)
                if (Buttons[i].Status)
                    Changed(i);
        }

        public void Changed(int number)
        {
            if (Active == number)
                return;

            Buttons[Active].Status = false;
            Buttons[number].Status = true;
            Active = number;

            foreach (RadioButton button in Buttons)
            {
                button.ChangeActive();
            }
        }
    }
}
