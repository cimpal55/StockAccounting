using StockAccounting.Checklist.Constants;
using StockAccounting.Checklist.Models.Data;
using StockAccounting.Checklist.Repositories.Interfaces;
using StockAccounting.Checklist.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services
{
    public class EmployeeDataService : IEmployeeDataService
    {
        private readonly IGenericRepository _repository;

        public EmployeeDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public Task<ObservableCollection<EmployeeDataModel>> GetEmployeeDataAsync()
        {
            UriBuilder uriBuilder = new UriBuilder(ApiConstants.BaseApiUrl)
            {
                Path = ApiConstants.EmployeeData
            };

            var employeeData = _repository.GetAsync<ObservableCollection<EmployeeDataModel>>(uriBuilder.ToString());

            return employeeData;
        }
    }
}
