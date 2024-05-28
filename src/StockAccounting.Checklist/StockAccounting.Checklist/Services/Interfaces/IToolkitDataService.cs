using StockAccounting.Checklist.Models.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services.Interfaces
{
    public interface IToolkitDataService
    {
        Task<ObservableCollection<ToolkitDataModel>> GetToolkitDataAsync();

        Task<ObservableCollection<ToolkitExternalDataModel>> GetToolkitExternalData(int id);
    }
}
