namespace StockAccounting.Inventory.Utils.Behaviors
{
    public sealed class BarcodeEntryCompleteBehavior : Behavior<Entry>
    {
        public int BarcodeLength { get; set; }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Completed += OnEntryComplete;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Completed -= OnEntryComplete;
        }

        void OnEntryComplete(object sender, EventArgs e)
        {
            var entry = (Entry)sender;

            if (entry.Text.Length == BarcodeLength)
                return;

            entry.Text = string.Empty;
            entry.Focus();
        }
    }
}
