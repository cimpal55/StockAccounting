using StockAccounting.Web.Constants;
using StockAccounting.Web.Services.Interfaces;

namespace StockAccounting.Web.Services
{
    public class FileUploadService : IFileUploadService
    {
        public async Task<bool> UploadFile(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, WebConstants.UploadedDir));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    await using var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create);
                    await file.CopyToAsync(fileStream);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }
    }
}
