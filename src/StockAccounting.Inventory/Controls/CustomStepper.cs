using System.Text.RegularExpressions;
using Microsoft.Maui.Devices;

namespace StockAccounting.Inventory.Controls
{
    public class CustomStepper : StackLayout
    {
        Button PlusBtn;
        Button MinusBtn;
        Entry Entry;
        string _previousValidText;

        public static readonly BindableProperty TextProperty =
          BindableProperty.Create(
             propertyName: "Text",
             returnType: typeof(decimal),
             declaringType: typeof(CustomStepper),
             defaultValue: 1m,
             defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty MinimumValueProperty =
            BindableProperty.Create("MinimumValue", typeof(decimal), typeof(CustomStepper), defaultValue: 0.1m);

        public decimal Text
        {
            get { return (decimal)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); OnPropertyChanged(nameof(Text)); }
        }

        public decimal MinimumValue
        {
            get { return (decimal)GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }
        public CustomStepper()
        {
            PlusBtn = new Button { Text = "+", WidthRequest = 50, FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = Colors.Green };
            MinusBtn = new Button { Text = "-", WidthRequest = 50, FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = Colors.Green };
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                PlusBtn.BackgroundColor = Colors.Transparent;
                MinusBtn.BackgroundColor = Colors.Transparent;
            }

            Orientation = StackOrientation.Horizontal;
            PlusBtn.Clicked += PlusBtn_Clicked;
            MinusBtn.Clicked += MinusBtn_Clicked;
            Entry = new Entry
            {
                
                PlaceholderColor = Colors.Gray,
                Keyboard = Keyboard.Numeric,          
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.Green,
                WidthRequest = 65,
                BackgroundColor = Colors.Transparent,
            };
            Entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
            Entry.TextChanged += Entry_TextChanged;
            Entry.Focused += Entry_Focused;
            Entry.Unfocused += Entry_Unfocused;
            Children.Add(MinusBtn);
            Children.Add(Entry);
            Children.Add(PlusBtn);
        }


        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;
            var text = e.NewTextValue;
            Match m = Regex.Match(text, "^[0-9]*(\\.[0-9]{0,2})?$");

            if (!m.Success)
            {
                string entryText = entry.Text;
                entryText = entryText.Remove(entryText.Length - 1);
                entry.Text = entryText;
            }
            else if(!string.IsNullOrEmpty(text))
            {
                if (0 < Convert.ToDecimal(text))
                    return;
                else
                    entry.Text = "0.1";
            }
        }

        private void Entry_Focused(object sender, EventArgs e)
        {
            var entry = (Entry)sender;
            _previousValidText = entry.Text;
            entry.Text = string.Empty;
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            if (string.IsNullOrWhiteSpace(entry.Text))
            {
                entry.Text = _previousValidText; 
            }
        }

        private void MinusBtn_Clicked(object sender, EventArgs e)
        {
            if (Text > 1)
                Text--;
            else if (Text <= 0.1m)
                Text = 0.1m;
            else
                Text = Text - 0.1m;
        }

        private void PlusBtn_Clicked(object sender, EventArgs e)
        {
            if (Text >= 1)
                Text++;
            else
                Text = Text + 0.1m;
        }

    }
}