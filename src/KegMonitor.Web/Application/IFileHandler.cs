using Microsoft.AspNetCore.Components.Forms;

namespace KegMonitor.Web.Application
{
    public interface IFileHandler
    {
        Task<string> UploadAsync(IBrowserFile file);
        Task DeleteAsync(string fileName);

    }
}
