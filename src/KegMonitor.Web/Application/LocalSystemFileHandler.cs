using Microsoft.AspNetCore.Components.Forms;

namespace KegMonitor.Web.Application
{
    public class LocalSystemFileHandler : IFileHandler
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _directory;

        public LocalSystemFileHandler(IWebHostEnvironment environment, string directory)
        {
            _environment = environment;
            _directory = directory;
        }

        private string GetAbsolutePath(string fileName)
        {
            return Path.Combine(_environment.WebRootPath, _directory, fileName);
        }

        public async Task<string> UploadAsync(IBrowserFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            string absolutePath = GetAbsolutePath(file.Name);

            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

            using (var inputStream = file.OpenReadStream(maxAllowedSize: 100000000))
            {
                using (var stream = new FileStream(absolutePath, FileMode.CreateNew))
                {
                    await inputStream.CopyToAsync(stream);
                }
            }

            return "/" + string.Join('/', _directory, file.Name);
        }

        public Task DeleteAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            string absolutePath = GetAbsolutePath(Path.GetFileName(fileName));

            if (!File.Exists(absolutePath))
                throw new InvalidOperationException("File not found.");

            File.Delete(absolutePath);

            return Task.CompletedTask;
        }
    }
}
