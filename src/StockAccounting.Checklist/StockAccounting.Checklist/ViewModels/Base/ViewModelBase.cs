using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sextant;
using StockAccounting.Checklist.Services.Interfaces;

namespace StockAccounting.Checklist.ViewModels.Base
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected readonly IDialogService _dialogService;

        public ViewModelBase(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        private bool _isBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual Task InitializeAsync(object data)
        {
            return Task.FromResult(false);
        }
    }
}
