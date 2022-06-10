using Microsoft.AspNetCore.Components.Forms;

namespace KegMonitor.Web.Application
{
    public class FileUploader : IFileUploader
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _directory;

        public FileUploader(IWebHostEnvironment environment, string directory)
        {
            _environment = environment;
            _directory = directory;
        }

        public async Task<string> UploadAsync(IBrowserFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            string absolutePath = Path.Combine(_environment.WebRootPath, _directory, file.Name);

            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

            using (var inputStream = file.OpenReadStream(maxAllowedSize: 100000000))
            {
                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    await inputStream.CopyToAsync(stream);
                }
            }

            return "/" + string.Join('/', _directory, file.Name);
        }
    }
}
