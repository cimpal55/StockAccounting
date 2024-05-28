using StockAccounting.Checklist.Constants;
using StockAccounting.Checklist.Models.Data;
using StockAccounting.Checklist.Repositories.Interfaces;
using StockAccounting.Checklist.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services
{
    public class ToolkitDataService : IToolkitDataService
    {
        private readonly IGenericRepository _repository;

        public ToolkitDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<ObservableCollection<ToolkitDataModel>> GetToolkitDataAsync()
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.ApiUrl)
            {
                Path = ApiConstants.ToolkitData
            };

            var toolkitData = await _repository.GetAsync<ObservableCollection<ToolkitDataModel>>(uriBuilder.ToString());

            return toolkitData;
        }

        public async Task<ObservableCollection<ToolkitExternalDataModel>> GetToolkitExternalData(int id)
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.ApiUrl)
            {
                Path = ApiConstants.ToolkitData + $"/{id}"
            };

            var toolkitExternalData = await _repository.GetAsync<ObservableCollection<ToolkitExternalDataModel>>(uriBuilder.ToString());

            return toolkitExternalData;
        }
    }
}
