using System.Net.Sockets;
using System.Net;
using static System.Net.WebRequestMethods;

namespace StockAccounting.Checklist.Constants
{
    public static class ApiConstants
    {
        public const string BaseApiUrl = "192.168.102.92";
        //public const string ApiUrl = "http://10.0.2.2:7088";
        public const string ApiUrl = "http://192.168.102.92:83";
        public const string InventoryData = "/api/inventorydata";
        public const string InventoryDataInsert = "/api/inventorydata/insertdocument";
        public const string EmployeeData = "/api/employeedata";
        public const string ExternalData = "/api/externaldata";
        public const string ExternalDataBarcode = "/api/externaldata/barcode=";
        public const string ExternalDataId = "/api/externaldata/id=";
        public const string ToolkitData = "api/toolkitdata";    
    }
}
