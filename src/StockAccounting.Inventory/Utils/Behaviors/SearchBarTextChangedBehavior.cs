namespace StockAccounting.Inventory.Utils.Behaviors
{
    public class SearchBarTextChangedBehavior : Behavior<SearchBar>
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
            if(e.NewTextValue.Length > 1 || e.NewTextValue.Length == 0)
                ((SearchBar)sender).SearchCommand?.Execute(e.NewTextValue);
        }
    }
}
