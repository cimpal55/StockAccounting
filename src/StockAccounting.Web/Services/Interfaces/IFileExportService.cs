using StockAccounting.Core.Data;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.Data.Excel;
using StockAccounting.Web.Models.Data;

namespace StockAccounting.Web.Services.Interfaces
{
    public interface IFileExportService
    {
        //public Task<IDocument>? CreateScannedDataCheckPdf(IEnumerable<DocumentModel> data, bool grouped);
        Task<ExportFileResponse> CreateScannedReportExcelFile(IEnumerable<int> idList, FileExport mode);
        Task<ExportFileResponse> CreateStockReportExcelFile(IEnumerable<int> idList, FileExport mode);
        Stream CreateScannedReportExcelStream(IEnumerable<ScannedReportExcelModel> data);
        Stream CreateStockReportExcelStream(IEnumerable<StockReportExcelModel> data);

    }
}
