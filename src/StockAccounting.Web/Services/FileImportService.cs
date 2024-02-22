using CsvHelper;
using CsvHelper.Configuration;
using StockAccounting.Core.Data.Repositories.Csv;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Web.Services.Interfaces;
using System.Globalization;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Web.Services
{
    public class FileImportService : IFileImportService
    {
        protected readonly IGenericRepository<ExternalDataModel> _repository;
        public FileImportService(IGenericRepository<ExternalDataModel> repository)
        {
            _repository = repository;
        }
        public async Task ImportNetSuiteCsvData(StreamReader stream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ",",
                IgnoreBlankLines = true
            };

            using var csvReader = new CsvReader(stream, config);
            csvReader.Context.RegisterClassMap<ExternalCsvMapModel>();
            var records = csvReader.GetRecords<ExternalCsvModel>();

            foreach (var record in records)
            {
                var item = new ExternalDataModel
                {
                    PluCode = record.PluCode,
                    ItemNumber = record.ItemNumber,
                    Barcode = record.Barcode,
                    Name = record.Name,
                    Unit = record.Unit,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                };

                await _repository.InsertAsync(item);

            }
        }
    }
}
