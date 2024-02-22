using StockAccounting.Checklist.Constants;
using StockAccounting.Checklist.Models.Data;
using StockAccounting.Checklist.Models.Data.DataTransferObjects;
using StockAccounting.Checklist.Repositories.Interfaces;
using StockAccounting.Checklist.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services
{
    public class InventoryDataService : IInventoryDataService
    {
        private readonly IGenericRepository _repository;
        public InventoryDataService(IGenericRepository repository)
        {
            _repository = repository;
        }
        public async Task<ObservableCollection<InventoryDataModel>> GetInventoryDataAsync()
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.BaseApiUrl)
            {
                Path = ApiConstants.InventoryData
            };

            var inventoryData = await _repository.GetAsync<ObservableCollection<InventoryDataModel>>(uriBuilder.ToString());

            return inventoryData;
        }

        public async Task<ScannedModel> InsertInventoryData(ScannedModel data)
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.BaseApiUrl)
            {
                Path = ApiConstants.InventoryDataInsert
            };

            var result = await _repository.PostAsync(uriBuilder.ToString(), data);

            return result;

        }
    }
}
