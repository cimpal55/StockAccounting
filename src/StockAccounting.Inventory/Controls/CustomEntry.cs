namespace StockAccounting.Inventory.Controls
{
    // Current stepper accept only int value.
    public class CustomEntry : StackLayout
    {
        Picker Picker;
        Entry Entry;

        public static readonly BindableProperty TextProperty =
          BindableProperty.Create(
             propertyName: "Text",
              returnType: typeof(int),
              declaringType: typeof(CustomEntry),
              defaultValue: 0,
              defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty MinimumValueProperty =
            BindableProperty.Create("MinimumValue", typeof(int), typeof(CustomEntry), defaultValue: 1);

        public int Text
        {
            get { return (int)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); OnPropertyChanged(nameof(Text)); }
        }

        public int MinimumValue
        {
            get { return (int)GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }
        public CustomEntry()
        {
            Picker = new Picker 
            { 
                Title = "Choose",
                WidthRequest = 40,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = Colors.Green,
                BackgroundColor = Colors.Transparent
            };

            Orientation = StackOrientation.Horizontal;
            Picker.SelectedIndexChanged += Picker_Changed;
            Entry = new Entry
            {
                PlaceholderColor = Colors.Gray,
                Keyboard = Keyboard.Numeric,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.Green,
                WidthRequest = 45,
                BackgroundColor = Colors.Transparent
            };
            Entry.ReturnType = ReturnType.Next;
            Entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
            Entry.TextChanged += Entry_TextChanged;
            Entry.Unfocused += Entry_Unfocused;
            Children.Add(Picker);
            Children.Add(Entry);
        }

        // Avoid decimal values
        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var text = ((Entry)sender).Text;
            if (string.IsNullOrEmpty(text) || text.Contains(","))
                Text = 0;
        }


        // Check if Minimum and Maximum value rules are respected.
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                try
                {
                    var entryValue = int.Parse(e.NewTextValue);
                    Text = entryValue;
                }
                catch (Exception)
                {
                    Text = 0;
                }
            }

        }

        private void Picker_Changed(object sender, EventArgs e)
        {
            Text++;
        }

    }
}