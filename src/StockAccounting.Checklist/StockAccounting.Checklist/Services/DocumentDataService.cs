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
    public class DocumentDataService : IDocumentDataService
    {
        private readonly IGenericRepository _repository;
        public DocumentDataService(IGenericRepository repository)
        {
            _repository = repository;
        }
        public async Task<ObservableCollection<DocumentDataModel>> GetDocumentDataAsync()
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.ApiUrl)
            {
                Path = ApiConstants.DocumentData
            };

            var documentData = await _repository.GetAsync<ObservableCollection<DocumentDataModel>>(uriBuilder.ToString());

            return documentData;
        }

        public async Task<ScannedModel> InsertDocumentData(ScannedModel data)
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.ApiUrl)
            {
                Path = ApiConstants.DocumentDataInsert
            };

            var result = await _repository.PostAsync(uriBuilder.ToString(), data);

            return result;

        }
    }
}
