using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using StockAccounting.Core.Data;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.Data.Excel;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Web.Extensions;
using StockAccounting.Web.Models.Data;
using StockAccounting.Web.Services.Interfaces;
using System.IO;

namespace StockAccounting.Web.Services
{
    public class FileExportService : IFileExportService
    {
        private readonly IEmployeeDataRepository _employeeDataRepository;
        private readonly IScannedDataRepository _scannedDataRepository;
        private readonly IStockDataRepository _stockDataRepository;
        public FileExportService(IEmployeeDataRepository employeeDataRepository,
            IScannedDataRepository scannedDataRepository,
            IStockDataRepository stockDataRepository)
        {
            _employeeDataRepository = employeeDataRepository;
            _scannedDataRepository = scannedDataRepository;
            _stockDataRepository = stockDataRepository;
        }

        public async Task<ExportFileResponse> CreateScannedReportExcelFile(IEnumerable<int> idList, FileExport mode)
        {

            var scannedReportData = await _scannedDataRepository.GetScannedReportDataAsync(idList, mode)
                                                                .ConfigureAwait(false);

            var scannedReportExcelModel = scannedReportData.Select(item => new ScannedReportExcelModel
            {
                Id = item.Id,
                Employee = _employeeDataRepository.GetEmployeeFullNameById(item.Id),
                ItemNumber = item.ItemNumber,
                Name = item.Name,
                PluCode = item.PluCode,
                Quantity = item.Quantity,
                Created = item.Created,
            });

            var stream = CreateScannedReportExcelStream(scannedReportExcelModel);

            return new ExportFileResponse(stream)
            {
                FileName = $"EmployeeReport_{mode}_{DateTime.Today:yyyy-MM-dd}.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
        public async Task<ExportFileResponse> CreateStockReportExcelFile(IEnumerable<int> idList, FileExport mode)
        {

            var stockReportData = await _stockDataRepository.GetStockReportDataAsync(idList, mode)
                                                            .ConfigureAwait(false);

            var stockReportExcelModel = stockReportData.Select(item => new StockReportExcelModel
            {
                Employee = item.Employee,
                StockName = item.StockName,
                Type = item.Type,
                Quantity = item.Quantity,
                Created = item.Created,
            });

            var stream = CreateStockReportExcelStream(stockReportExcelModel);

            return new ExportFileResponse(stream)
            {
                FileName = $"StockReport_{mode}_{DateTime.Today:yyyy-MM-dd}.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
        public Stream CreateStockReportExcelStream(IEnumerable<StockReportExcelModel> data)
        {
            var excelData = data.Select(x => x.StockName).Distinct().ToArray();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var excel = new ExcelPackage();

            foreach (var item in excelData)
            {
                var sheet = excel.Workbook.Worksheets.Add(item);

                using (var range = sheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                sheet.Cells[1, 1].Value = "ID";
                sheet.Cells[1, 2].Value = "Employee";
                sheet.Cells[1, 3].Value = "Type";
                sheet.Cells[1, 4].Value = "Quantity";
                sheet.Cells[1, 5].Value = "Date";

                var i = 2;

                foreach (var value in data.Where(x => x.StockName == item))
                {
                    sheet.Cells[i, 5].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";

                    sheet.Cells[i, 1].Value = i - 1;
                    sheet.Cells[i, 2].Value = value.Employee;
                    sheet.Cells[i, 3].Value = value.Type;
                    sheet.Cells[i, 4].Value = value.Quantity;
                    sheet.Cells[i, 5].Value = value.Created;

                    i++;
                }

                sheet.Cells[sheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells[sheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells[sheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells[sheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }

            var stream = new MemoryStream();
            excel.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }

        public Stream CreateScannedReportExcelStream(IEnumerable<ScannedReportExcelModel> data)
        {
            var excelData = data.Select(x => x.Employee).Distinct().ToArray();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var excel = new ExcelPackage();

            foreach(var item in excelData)
            {
                var sheet = excel.Workbook.Worksheets.Add(item);

                using (var range = sheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                sheet.Cells[1, 1].Value = "ID";
                sheet.Cells[1, 2].Value = "Name";
                sheet.Cells[1, 3].Value = "ItemNumber";
                sheet.Cells[1, 4].Value = "PluCode";
                sheet.Cells[1, 5].Value = "Quantity";
                sheet.Cells[1, 6].Value = "Date";

                var i = 2;

                foreach (var value in data.Where(x => x.Employee == item))
                {
                    sheet.Cells[i, 6].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";

                    sheet.Cells[i, 1].Value = i - 1;
                    sheet.Cells[i, 2].Value = value.Name;
                    sheet.Cells[i, 3].Value = value.ItemNumber;
                    sheet.Cells[i, 4].Value = value.PluCode;
                    sheet.Cells[i, 5].Value = value.Quantity;
                    sheet.Cells[i, 6].Value = value.Created;

                    i++;
                }

                sheet.Cells[sheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells[sheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells[sheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells[sheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }

            var stream = new MemoryStream();
            excel.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }
    }
}
