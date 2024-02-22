using Acr.UserDialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StockAccounting.Checklist.Utility
{
    public class EntryFocusBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty IsCorrectProperty =
                BindableProperty.Create("IsCorrect", typeof(bool), typeof(EntryFocusBehavior), null);
        public bool IsCorrect
        {
            get { return (bool)GetValue(IsCorrectProperty); }
            set { SetValue(IsCorrectProperty, value); }
        }
        public string NextFocusElementName { get; set; }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.BindingContextChanged += (sender, _) => this.BindingContext = ((BindableObject)sender).BindingContext;
            bindable.Completed += Bindable_Completed;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.Completed -= Bindable_Completed;
            base.OnDetachingFrom(bindable);
        }

        private void Bindable_Completed(object sender, EventArgs e)
        {
            var entry = (Entry)sender;
         
            if (string.IsNullOrEmpty(NextFocusElementName))
            {
                entry.Focus();
                return;
            }

            var parent = entry.Parent;
            while (parent != null)
            {
                var nextFocusElement = parent.FindByName<Entry>(NextFocusElementName);
                if (nextFocusElement != null)
                {                
                    nextFocusElement.Focus();
                    break;
                }
                else
                {
                    parent = parent.Parent;
                }
            }
        }
    }
}
