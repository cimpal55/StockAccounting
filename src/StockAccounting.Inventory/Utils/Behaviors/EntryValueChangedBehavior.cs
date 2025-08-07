namespace StockAccounting.Inventory.Utils.Behaviors
{
    public class EntryValueChangedBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Focused += Entry_Focused;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Focused -= Entry_Focused;
        }

        private void Entry_Focused(object sender, EventArgs e)
        {
            var entry = (Entry)sender;

            if (!string.IsNullOrEmpty(entry.Text) && (!int.TryParse(entry.Text, out var intVal) || intVal <= 0))
                entry.Text = null;

            entry.Focus();
        }
    }
}
