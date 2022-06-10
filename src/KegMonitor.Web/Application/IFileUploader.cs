using Microsoft.AspNetCore.Components.Forms;

namespace KegMonitor.Web.Application
{
    public interface IFileUploader
    {
        Task<string> UploadAsync(IBrowserFile file);
    }
}
