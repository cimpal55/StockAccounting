namespace StockAccounting.Inventory.Utils.Triggers
{
    public class SearchBarFocusTrigger : TriggerAction<SearchBar>
    {
        protected override void Invoke(SearchBar entry)
        {
            entry.Focus();
        }
    }
}
