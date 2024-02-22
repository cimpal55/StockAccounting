using Acr.UserDialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StockAccounting.Checklist.Utility
{
    public class PickerFocusBehavior : Behavior<Picker>
    {
        public string NextFocusElementName { get; set; }

        protected override void OnAttachedTo(Picker bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Unfocused += Bindable_Completed;
        }

        protected override void OnDetachingFrom(Picker bindable)
        {
            bindable.PropertyChanged -= Bindable_Completed;
            base.OnDetachingFrom(bindable);
        }

        private void Bindable_Completed(object sender, EventArgs e)
        {
            var entry = (Picker)sender;

            if (string.IsNullOrEmpty(NextFocusElementName) || entry.SelectedItem == null)
            {
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
