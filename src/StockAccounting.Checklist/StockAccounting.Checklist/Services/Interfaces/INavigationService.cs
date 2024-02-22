using StockAccounting.Checklist.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services.Interfaces
{
    public interface INavigationService
    {
        Task InitializeAsync();

        Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase;

        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        Task NavigateToAsync(Type viewModelType);

        Task ClearBackStack();

        Task NavigateToAsync(Type viewModelType, object parameter);

        Task NavigateBackAsync();

    }
}
