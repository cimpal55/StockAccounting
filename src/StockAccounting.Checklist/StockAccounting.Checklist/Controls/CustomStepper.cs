using DynamicData;
using Newtonsoft.Json.Converters;
using Sextant;
using StockAccounting.Checklist.Extensions;
using StockAccounting.Checklist.Models.Data;
using StockAccounting.Checklist.Utility.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StockAccounting.Checklist.Controls
{
    public class CustomStepper : StackLayout
    {
        Button PlusBtn;
        Button MinusBtn;
        Entry Entry;

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
            PlusBtn = new Button { Text = "+", WidthRequest = 50, FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = Color.Green };
            MinusBtn = new Button { Text = "-", WidthRequest = 50, FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = Color.Green };
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                {
                    PlusBtn.BackgroundColor = Color.Transparent;
                    MinusBtn.BackgroundColor = Color.Transparent;
                    break;
                }
            }

            Orientation = StackOrientation.Horizontal;
            PlusBtn.Clicked += PlusBtn_Clicked;
            MinusBtn.Clicked += MinusBtn_Clicked;
            Entry = new Entry
            {
                
                PlaceholderColor = Color.Gray,
                Keyboard = Keyboard.Numeric,          
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Green,
                WidthRequest = 65,
                BackgroundColor = Color.Transparent,
            };
            Entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
            Entry.TextChanged += Entry_TextChanged;
            Entry.Focused += Entry_Focused;
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
            entry.Text = string.Empty;
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