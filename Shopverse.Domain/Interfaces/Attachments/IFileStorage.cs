using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Domain.Interfaces.Attachments
{
    public interface IFileStorage
    {
        Task<(bool IsSuccess, string? FilePath, string? ErrorMessage)> SaveFileAsyncSafe(
                   Stream fileStream, string fileName, string contentType, CancellationToken ct);
        Task DeleteFileAsync(string filePath, CancellationToken ct);
    }
}
