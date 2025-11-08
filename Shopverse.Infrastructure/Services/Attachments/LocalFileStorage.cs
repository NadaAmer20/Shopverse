using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Interfaces.Attachments;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Infrastructure.Services.Attachments
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly IRepository<Setting> _settingsRepo;
        private string _basePath = "uploads";
        private long _maxUploadSize = 5_000_000;
        private string[] _allowedExtensions = new[]
        {
            ".jpg", ".jpeg", ".png", ".gif", ".svg", ".bmp", ".tiff", ".webp",
            ".mp4", ".mov", ".avi", ".pdf", ".docx", ".xlsx"
        };
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalFileStorage(IRepository<Setting> settingsRepo, IHttpContextAccessor httpContextAccessor)
        {
            _settingsRepo = settingsRepo;
            _httpContextAccessor = httpContextAccessor;

            // Load settings
            var pathSetting = _settingsRepo.GetQueryable().FirstOrDefault(s => s.Key == "UploadsPath");
            if (pathSetting != null && !string.IsNullOrWhiteSpace(pathSetting.Value))
                _basePath = pathSetting.Value;

            var maxSizeSetting = _settingsRepo.GetQueryable().FirstOrDefault(s => s.Key == "MaxUploadSize");
            if (maxSizeSetting != null && long.TryParse(maxSizeSetting.Value, out long size))
                _maxUploadSize = size;

            var extSetting = _settingsRepo.GetQueryable().FirstOrDefault(s => s.Key == "FileAllowedExtensions");
            if (extSetting != null && !string.IsNullOrWhiteSpace(extSetting.Value))
                _allowedExtensions = extSetting.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            Directory.CreateDirectory(_basePath);
        }

        public async Task<(bool IsSuccess, string? FilePath, string? ErrorMessage)> SaveFileAsyncSafe(
            Stream fileStream, string fileName, string contentType, CancellationToken ct)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (ext == null || !_allowedExtensions.Contains(ext))
            {
                return (false, null, $"Invalid file extension. Allowed: {string.Join(", ", _allowedExtensions)}");
            }

            if (fileStream.Length > _maxUploadSize)
            {
                return (false, null, $"File too large. Maximum allowed size is {_maxUploadSize} bytes.");
            }

            var safeName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
            var folder = Path.Combine(_basePath, DateTime.UtcNow.ToString("yyyyMMdd"));
            Directory.CreateDirectory(folder);
            var full = Path.Combine(folder, safeName);

            using var fs = File.Create(full);
            await fileStream.CopyToAsync(fs, ct);

            return (true, full, null);
        }

        public Task DeleteFileAsync(string filePath, CancellationToken ct)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            return Task.CompletedTask;
        }
    }
}
