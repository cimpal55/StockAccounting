using StockAccounting.Inventory.Models;
using StockAccounting.Inventory.ViewModels;

#if ANDROID
using Android.Widget;
using AndroidX.AppCompat.Widget;
#endif

namespace StockAccounting.Inventory.Views
{
    public partial class ScannedInventoryDataView : ViewBase
    {
        bool _keyboardVisible = false;
        private ScannedInventoryDataViewModel? _currentViewModel;

        public ScannedInventoryDataView(ScannedInventoryDataViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        } 

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(70);
            HideKeyboard();
            documentSearchBar.Focus();
        }

        private void OnToggleKeyboardClicked(object sender, EventArgs e)
        {
            if (_keyboardVisible)
            {
                HideKeyboard();
                _keyboardVisible = false;
                documentSearchBar.Focus();
            }
            else
            {
                ShowKeyboard();
                _keyboardVisible = true;
                documentSearchBar.Focus();
            }
        }

        private void OnSearchBoxFocused(object sender, FocusEventArgs e)
        {
            if (!_keyboardVisible)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(200);
                    DisableKeyboard();
                    HideKeyboard();
                });
            }
        }

        private void DisableKeyboard()
        {
#if ANDROID
            if (documentSearchBar.Handler?.PlatformView is AndroidX.AppCompat.Widget.SearchView searchView)
            {
                var editText = searchView.FindViewById<EditText>(Resource.Id.search_src_text);
                if (editText != null)
                {
                    editText.ShowSoftInputOnFocus = false;
                }
            }
            else if (documentSearchBar.Handler?.PlatformView is EditText editText)
            {
                editText.ShowSoftInputOnFocus = false;
            }
#endif
        }
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
#if ANDROID
            if (!_keyboardVisible)
            {
                if (string.IsNullOrEmpty(e.NewTextValue) && !string.IsNullOrEmpty(e.OldTextValue))
                {
                    var handler = documentSearchBar.Handler;
                    if (handler?.PlatformView is AndroidX.AppCompat.Widget.SearchView searchView)
                    {
                        var editText = searchView.FindViewById<EditText>(Resource.Id.search_src_text);
                        if (editText != null)
                        {
                            editText.ClearFocus();

                            var inputManager = Platform.CurrentActivity?.GetSystemService(Android.Content.Context.InputMethodService)
                                as Android.Views.InputMethods.InputMethodManager;

                            inputManager?.HideSoftInputFromWindow(editText.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);

                            editText.ShowSoftInputOnFocus = false;
                        }
                    }
                }
            }
#endif
        }


        private void ShowKeyboard()
        {
#if ANDROID
            EditText editText = null;

            if (documentSearchBar.Handler?.PlatformView is AndroidX.AppCompat.Widget.SearchView searchView)
            {
                editText = searchView.FindViewById<EditText>(Resource.Id.search_src_text);
            }
            else if (documentSearchBar.Handler?.PlatformView is EditText directEditText)
            {
                editText = directEditText;
            }

            if (editText != null)
            {
                editText.ShowSoftInputOnFocus = true;
                var context = Platform.CurrentActivity;
                var inputManager = context?.GetSystemService(Android.Content.Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
                inputManager?.ShowSoftInput(editText, Android.Views.InputMethods.ShowFlags.Implicit);
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
#endif
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is ScannedInventoryDataViewModel vm)
            {
                vm.ScrollToRequested += item =>
                {
                    collectionView.ScrollTo(item, position: ScrollToPosition.Start, animate: true);
                };

                vm.FocusSearchRequested += () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        documentSearchBar.Focus();
                        documentSearchBar.CursorPosition = 0;
                    });
                };
            }
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            documentSearchBar.Focus();
        }

    }
}
