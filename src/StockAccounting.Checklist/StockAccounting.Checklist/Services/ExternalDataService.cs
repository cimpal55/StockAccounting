using StockAccounting.Checklist.Constants;
using StockAccounting.Checklist.Models.Data;
using StockAccounting.Checklist.Repositories.Interfaces;
using StockAccounting.Checklist.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services
{
    public class ExternalDataService : IExternalDataService
    {
        private readonly IGenericRepository _repository;
        public ExternalDataService(IGenericRepository repository)
        {
            _repository = repository;
        }
        public Task<ObservableCollection<ExternalDataModel>> GetExternalDataAsync()
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.BaseApiUrl)
            {
                Path = ApiConstants.ExternalData
            };

            var externalData = _repository.GetAsync<ObservableCollection<ExternalDataModel>>(uriBuilder.ToString());

            return externalData;
        }

        public Task<ExternalDataModel> GetExternalDataByBarcodeAsync(string barcode)
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.BaseApiUrl)
            {
                Path = ApiConstants.ExternalData + $"/{barcode}"
            };

            var externalData = _repository.GetAsync<ExternalDataModel>(uriBuilder.ToString());

            return externalData;
        }
    }
}
