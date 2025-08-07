using CommunityToolkit.Maui.Core.Platform;
using StockAccounting.Inventory.Utility;
using StockAccounting.Inventory.ViewModels;
#if ANDROID
using Android.Widget;
using AndroidX.AppCompat.Widget;
#endif
namespace StockAccounting.Inventory.Views
{
    public partial class ScannedInventoryDataAddView : ViewBase
    {
        private bool _keyboardVisible = false;
        public ScannedInventoryDataAddView(ScannedInventoryDataAddViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(70);
            HideKeyboard();
            barcodeEntry.Focus();
            barcodeEntry.CursorPosition = 0;
        }
        private void OnToggleKeyboardClicked(object sender, EventArgs e)
        {
            if (_keyboardVisible)
            {
                HideKeyboard();
            }
            else
            {
                ShowKeyboard();
                barcodeEntry.Focus();
            }
        }
        private void OnEntryFocused(object sender, FocusEventArgs e)
        {
            if (!_keyboardVisible && e.IsFocused)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    DisableKeyboard();
                    HideKeyboard();
                });
            }
        }
        private void OnBarcodeTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue) && !_keyboardVisible)
            {
                barcodeEntry.Unfocus();
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    DisableKeyboard();
                    HideKeyboard();
                    barcodeEntry.Focus();
                });
            }
        }

        private void DisableKeyboard()
        {
#if ANDROID
            if (barcodeEntry.Handler?.PlatformView is AndroidX.AppCompat.Widget.AppCompatEditText editText)
            {
                editText.ShowSoftInputOnFocus = false;
            }
#endif
        }
        private void ShowKeyboard()
        {
#if ANDROID
            if (barcodeEntry.Handler?.PlatformView is AndroidX.AppCompat.Widget.AppCompatEditText editText)
            {
                editText.ShowSoftInputOnFocus = true;
                var context = Platform.CurrentActivity;
                var inputManager = context?.GetSystemService(Android.Content.Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
                inputManager?.ShowSoftInput(editText, Android.Views.InputMethods.ShowFlags.Implicit);
                _keyboardVisible = true;
            }
#endif
        }
        private void HideKeyboard()
        {
#if ANDROID
            var context = Platform.CurrentActivity;
            var inputManager = context?.GetSystemService(Android.Content.Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
            var token = context?.CurrentFocus?.WindowToken;
            inputManager?.HideSoftInputFromWindow(token, Android.Views.InputMethods.HideSoftInputFlags.None);
            DisableKeyboard();
            _keyboardVisible = false;
#endif
        }
    }
}