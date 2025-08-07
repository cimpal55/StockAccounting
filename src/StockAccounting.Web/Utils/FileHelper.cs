namespace StockAccounting.Web.Utils
{
    public static class FileHelper
    {
        public static bool IsFileInUseGeneric(FileInfo file)
        {
            try
            {
                using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }
    }
}
