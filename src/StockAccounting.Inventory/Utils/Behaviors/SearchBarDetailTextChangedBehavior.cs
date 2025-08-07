namespace StockAccounting.Inventory.Utils.Behaviors
{
    public class SearchBarDetailTextChangedBehavior : Behavior<SearchBar>
    {
        protected override void OnAttachedTo(SearchBar bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += this.SearchBar_TextChanged;
        }

        protected override void OnDetachingFrom(SearchBar bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= this.SearchBar_TextChanged;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 3 || e.NewTextValue.Length == 0 || e.NewTextValue.Length != 13)
                ((SearchBar)sender).SearchCommand?.Execute(e.NewTextValue);
        }
    }
}