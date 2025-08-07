using CommunityToolkit.Mvvm.ComponentModel;

namespace StockAccounting.Core.Android.Models.Base
{
    public abstract class ListItemBase<TKey> : ObservableObject
    {
        protected ListItemBase(TKey key)
        {
            Key = key;
        }

        public TKey Key { get; }
    }
}