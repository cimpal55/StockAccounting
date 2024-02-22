namespace StockAccounting.Web.Services.Interfaces
{
    public interface IFileUploadService
    {
        public Task<bool> UploadFile(IFormFile file);
    }
}
