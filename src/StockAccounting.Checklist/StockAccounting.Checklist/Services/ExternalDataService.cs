using StockAccounting.Checklist.Constants;
using StockAccounting.Checklist.Models.Data;
using StockAccounting.Checklist.Repositories.Interfaces;
using StockAccounting.Checklist.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode.Internal;

namespace StockAccounting.Checklist.Services
{
    public class ExternalDataService : IExternalDataService
    {
        private readonly IGenericRepository _repository;
        public ExternalDataService(IGenericRepository repository)
        {
            _repository = repository;
        }
        public async Task<ObservableCollection<ExternalDataModel>> GetExternalDataAsync()
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.ApiUrl)
            {
                Path = ApiConstants.ExternalData
            };

            var externalData = await _repository.GetAsync<ObservableCollection<ExternalDataModel>>(uriBuilder.ToString());

            return externalData;
        }

        public async Task<ExternalDataModel> GetExternalDataByBarcodeAsync(string barcode)
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.ApiUrl)
            {
                Path = ApiConstants.ExternalDataBarcode + barcode
            };

            var externalData = await _repository.GetAsync<ExternalDataModel>(uriBuilder.ToString());

            return externalData;
        }

        public async Task<ExternalDataModel> GetExternalDataByIdAsync(int externalDataId) {

            UriBuilder uriBuilder = new UriBuilder(ApiConstants.ApiUrl)
            {
                Path = ApiConstants.ExternalDataId + externalDataId
            };

            var externalData = await _repository.GetAsync<ExternalDataModel>(uriBuilder.ToString());

            return externalData;
        }
    }
}
