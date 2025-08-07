using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using Acr.UserDialogs;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Inventory.Models;
using StockAccounting.Inventory.Services.Interfaces;
using static LinqToDB.DataProvider.SqlServer.SqlFn;

namespace StockAccounting.Inventory.Services
{
    public class RestService : IRestService
    {
        private static readonly string RestUrl = Preferences.Get("RestUrl", "default_value");
        private readonly HttpClient _httpClient = new();

        public RestService()
        { }
        
        protected async Task<string> SendDataAsync(string url, string jsonData)
        {
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<IReadOnlyList<T>?> GetDataAsync<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IReadOnlyList<T>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        protected async Task<T?> GetSingleDataAsync<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        protected async Task<IReadOnlyList<T>?> SendAndReceiveDataAsync<T>(string url, string jsonData)
        {
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IReadOnlyList<T>>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        protected async Task<T?> SendAndReceiveSingleDataAsync<T>(string url, string jsonData)
        {
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task<bool> CheckConnectionAsync(string ip)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(ip, 3000);

                if (reply.Status == IPStatus.Success)
                    return true;

                UserDialogs.Instance.Toast(new ToastConfig("No access! IP not available. Check your VPN")
                    .SetBackgroundColor(System.Drawing.Color.Red)
                    .SetPosition(ToastPosition.Top));

                return false;
            }
            catch (Exception)
            {
                UserDialogs.Instance.Toast(new ToastConfig("No internet! Please check your connection.")
                    .SetBackgroundColor(System.Drawing.Color.Red)
                    .SetPosition(ToastPosition.Top));

                return false;
            }
        }


    public async Task<string> SendScannedDataUpdateAsync(string jsonData, int docId)
        {
            var url = $"{RestUrl}ScannedInventoryData/UpdateDocumentDetails/{docId}";
            return await SendDataAsync(url, jsonData);
        }

        public async Task<string> SendScannedDataInsertAsync(string jsonData, int docId)
        {
            var url = $"{RestUrl}ScannedInventoryData/InsertDocumentDetails/{docId}";
            return await SendDataAsync(url, jsonData);
        }

        public async Task<IReadOnlyList<ScannedInventoryDataJson>> GetDetailsByEmployeeId(int id)
        {
            var url = $"{RestUrl}ScannedInventoryData/employeeId={id}";
            return await GetDataAsync<ScannedInventoryDataJson>(url);
        }

        public async Task<int> InsertInventoryDataAsync(string jsonData)
        {
            var url = $"{RestUrl}InventoryData/InsertInventory";
            return await SendAndReceiveSingleDataAsync<int>(url, jsonData);
        }

        public async Task<string> SendInventoryDataAsync(string jsonData)
        {
            var url = $"{RestUrl}InventoryData/UpdateDocument";
            return await SendDataAsync(url, jsonData);
        }

        public async Task<IReadOnlyList<ScannedInventoryDataJson>> GetDetListByDocIdAsync(int docId)
        {
            var url = $"{RestUrl}ScannedInventoryData/DocId={docId}";
            return await GetDataAsync<ScannedInventoryDataJson>(url);
        }

        public async Task<int> GetInventoryDataIdByName(string docName)
        {
            var url = $"{RestUrl}InventoryData/Name={docName}";
            return await GetSingleDataAsync<int>(url);
        }

        public async Task<int> CreateDocumentAfterInventoryCheck(string jsonData)
        {
            var url = $"{RestUrl}DocumentData/InsertDocumentAfterInventoryCheck";
            return await SendAndReceiveSingleDataAsync<int>(url, jsonData);
        }

        public async Task CreateDetailsAfterInventoryCheck(string jsonData, int docId)
        {
            var url = $"{RestUrl}DocumentData/InsertDetailsAfterInventoryCheck/docId={docId}";
            await SendDataAsync(url, jsonData);
        }

        public async Task<IReadOnlyList<InventoryDataJson>?> GetInventoryDataAsync()
        {
            var url = $"{RestUrl}InventoryData/";
            return await GetDataAsync<InventoryDataJson>(url);
        }

        public async Task<IReadOnlyList<InventoryDataJson>?> GetLatestInventoryDataAsync(string jsonData)
        {
            var url = $"{RestUrl}InventoryData/GetLatestInventoryData";
            return await SendAndReceiveDataAsync<InventoryDataJson>(url, jsonData);
        }

        public async Task<IReadOnlyList<InventoryDataJson>?> GetCheckedInventoryDataAsync()
        {
            var url = $"{RestUrl}InventoryData/GetCheckedInventoryData";
            return await GetDataAsync<InventoryDataJson>(url);
        }

        public async Task<IReadOnlyList<ScannedInventoryDataJson>?> GetLatestScannedDataAsync(string jsonData)
        {
            var url = $"{RestUrl}ScannedInventoryData/GetLatestScannedData";
            return await SendAndReceiveDataAsync<ScannedInventoryDataJson>(url, jsonData);
        }

        public async Task<IReadOnlyList<ScannedInventoryDataJson>?> GetScannedDataAsync()
        {
            var url = $"{RestUrl}ScannedInventoryData/";
            return await GetDataAsync<ScannedInventoryDataJson>(url);
        }

        public async Task<IReadOnlyList<EmployeeJson>?> GetEmployeesAsync()
        {
            var url = $"{RestUrl}EmployeeData/";
            return await GetDataAsync<EmployeeJson>(url);
        }

        public async Task<IReadOnlyList<InventoryDataJson>?> GetInprocessInventoryDataAsync()
        {
            var url = $"{RestUrl}InventoryData/GetInprocessInventoryData";
            return await GetDataAsync<InventoryDataJson>(url);
        }

        //public async Task<InventoryDataJson?> InsertScannedEmployeeInventoryData(int id)
        //{
        //    var url = $"{RestUrl}ScannedInventoryData/GetLatestScannedData/employeeId={id}";
        //    return await SendAndReceiveDataAsync<InventoryDataJson>(url, jsonData);
        //}

        public async Task<ExternalDataRecord> GetExternalDataIdByBarcode(string barcode)
        {
            var url = $"{RestUrl}ExternalData/barcode={barcode}";
            return await GetSingleDataAsync<ExternalDataRecord>(url);
        }

        public async Task<ExternalDataModel> GetExternalDataById(int id)
        {
            var url = $"{RestUrl}ExternalData/id={id}";
            return await GetSingleDataAsync<ExternalDataModel>(url);
        }

        public async Task<ObservableCollection<ExternalDataModel>> GetExternalData()
        {
            var url = $"{RestUrl}ExternalData";
            var list = await GetDataAsync<ExternalDataModel>(url);

            return list != null
                ? new ObservableCollection<ExternalDataModel>(list)
                : [];
        }
    }
}
