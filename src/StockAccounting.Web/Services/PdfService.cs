//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;
//using StockAccounting.Core.Data.Models.Data;
//using StockAccounting.Web.Services.Interfaces;

//namespace StockAccounting.Web.Services
//{
//    public class PdfService : IPdfService
//    {


//        public Task<IDocument> CreateEmployeePdf(IEnumerable<StockDataModel> data, string docType)
//        {
//            var document = Document.Create(container =>
//            {
//                container.Page(page =>
//                {
//                    page.Size(PageSizes.A4);
//                    page.PageColor(Colors.White);
//                    page.MarginHorizontal(1f, Unit.Centimetre);
//                    page.DefaultTextStyle(static x => x.FontSize(15));
//                    page.DefaultTextStyle(x => x.FontFamily("Arial"));


//                });
//            });
//        }
//    }
//}
