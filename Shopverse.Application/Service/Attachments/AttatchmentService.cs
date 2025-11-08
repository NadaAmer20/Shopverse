using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shopverse.Application.DTOs.Attachments;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Enums;
using Shopverse.Domain.Interfaces.Attachments;


namespace Shopverse.Application.Services.Attachments
{
    public class AttachmentService
    {
        private readonly IRepository<Attachment> _attachmentRepo;
        private readonly IFileStorage _fileStorage;
        private readonly IRepository<Setting> _settingsRepo;

        public AttachmentService(
            IRepository<Attachment> attachmentRepo,
            IFileStorage fileStorage,
            IRepository<Setting> settingsRepo)
        {
            _attachmentRepo = attachmentRepo;
            _fileStorage = fileStorage;
            _settingsRepo = settingsRepo;
        }

        #region Helpers

        private long MaxUploadSize
        {
            get
            {
                var setting = _settingsRepo.GetQueryable()
                    .FirstOrDefault(s => s.Key == "MaxUploadSize");

                return setting != null && long.TryParse(setting.Value, out var size)
                    ? size
                    : 5_000_000;
            }
        }

        #endregion

        #region Add Attachments
        public async Task UploadMultipleFormatsAsync(
    IEnumerable<IFormFile> files, Guid entityId, EntityTypeEnum type, CancellationToken cancellationToken)
        {
            foreach (var file in files)
            {
                await UploadSingleAsync(file, entityId, null, type, cancellationToken);
            }
        }

        public async Task<ApiResponse<AttachmentDto>> UploadSingleAsync(
     IFormFile file,
     Guid attachmentableId,
     Guid? userId,
     EntityTypeEnum attachmentableType,
     CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return ApiResponse<AttachmentDto>.Fail("No file provided");

            var uploadResult = await SaveFileAsync(file, ct);
            if (!uploadResult.IsSuccess || uploadResult.Data == null)
                return ApiResponse<AttachmentDto>.Fail(uploadResult.Message ?? "Failed to save file");

            var attachment = CreateAttachmentEntity(
                file,
                uploadResult.Data,
                attachmentableId,
                userId,
                attachmentableType
            );

            await _attachmentRepo.AddAsync(attachment, ct);

            var dto = new AttachmentDto(attachment);

            return ApiResponse<AttachmentDto>.Success(dto, "File uploaded successfully");
        }

        public async Task<ApiResponse<List<AttachmentDto>>> AddAttachmentsAsync(
            List<AttachmentFormModel>? attachments,
            Guid attachmentableId,
            Guid? userId,

            CancellationToken ct)
        {
            if (attachments?.Any() != true)
                return ApiResponse<List<AttachmentDto>>.Fail("No files provided");

            var result = new List<AttachmentDto>();
            var errors = new List<string>();

            foreach (var model in attachments)
            {
                var file = model.File;
                if (file == null || file.Length == 0)
                    continue;

                //if (file.Length > MaxUploadSize)
                //{
                //    errors.Add($"File {file.FileName} exceeds maximum allowed size ({MaxUploadSize} bytes)");
                //    continue;
                //}

                var uploadResult = await SaveFileAsync(file, ct);
                if (!uploadResult.IsSuccess)
                {
                    errors.Add(uploadResult.Message);
                    continue;
                }

                var attachment = CreateAttachmentEntity(
                    file,
                    uploadResult.Data!,
                    attachmentableId,
                    userId,
                    model.AttachmentableType
                );

                await _attachmentRepo.AddAsync(attachment, ct);
                result.Add(new AttachmentDto(attachment));
            }

            if (errors.Any())
                return ApiResponse<List<AttachmentDto>>.Fail(string.Join("; ", errors));

            return ApiResponse<List<AttachmentDto>>.Success(result, "Files uploaded successfully");
        }
        public async Task<ApiResponse<List<AttachmentDto>>> AddAttachmentsAsync(
           List<IFormFile>? attachments,
           Guid attachmentableId,
           Guid? userId,
           EntityTypeEnum entityTypeEnum,
           CancellationToken ct)
        {
            if (attachments?.Any() != true)
                return ApiResponse<List<AttachmentDto>>.Fail("No files provided");

            var result = new List<AttachmentDto>();
            var errors = new List<string>();

            foreach (var file in attachments)
            {
                if (file == null || file.Length == 0)
                    continue;
                var uploadResult = await SaveFileAsync(file, ct);
                if (!uploadResult.IsSuccess)
                {
                    errors.Add(uploadResult.Message);
                    continue;
                }

                var attachment = CreateAttachmentEntity(
                    file,
                    uploadResult.Data!,
                    attachmentableId,
                    userId,
                    entityTypeEnum);

                await _attachmentRepo.AddAsync(attachment, ct);
                result.Add(new AttachmentDto(attachment));
            }

            if (errors.Any())
                return ApiResponse<List<AttachmentDto>>.Fail(string.Join("; ", errors));

            return ApiResponse<List<AttachmentDto>>.Success(result, "Files uploaded successfully");
        }
        public async Task<ApiResponse<AttachmentDto>> AddAttachmentAsync(
            IFormFile file,
            Guid? createdById,
            Guid? attachmentableId, EntityTypeEnum AttachmentableType,
            CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return ApiResponse<AttachmentDto>.Fail("No file provided");

            if (file.Length > MaxUploadSize)
                return ApiResponse<AttachmentDto>.Fail($"File exceeds max size ({MaxUploadSize} bytes)");

            var uploadResult = await SaveFileAsync(file, ct);
            if (!uploadResult.IsSuccess)
                return ApiResponse<AttachmentDto>.Fail(uploadResult.Message);

            var attachment = CreateAttachmentEntity(
                file,
                uploadResult.Data!,
                attachmentableId ?? Guid.Empty,
                createdById,
                AttachmentableType
            );

            await _attachmentRepo.AddAsync(attachment, ct);

            return ApiResponse<AttachmentDto>.Success(new AttachmentDto(attachment), "Attachment uploaded successfully");
        }

        #endregion

        #region Remove Attachments

        public async Task RemoveAttachmentsAsync(List<Guid>? attachmentIds, CancellationToken ct)
        {
            if (attachmentIds?.Any() != true)
                return;

            var attachments = await _attachmentRepo.GetQueryable()
                .Where(a => attachmentIds.Contains(a.AttachmentableId))
                .ToListAsync(ct);

            foreach (var att in attachments)
            {
                await _fileStorage.DeleteFileAsync(att.FilePath, ct);
                await _attachmentRepo.RemoveAsync(att, ct);
            }
        }

        #endregion

        #region Get Attachments

        public async Task<List<AttachmentDto>> GetAttachmentsAsync(Guid attachmentableId, CancellationToken ct)
        {
            return await _attachmentRepo.GetQueryable()
                .Where(a => a.AttachmentableId == attachmentableId)
                .Select(a => new AttachmentDto(a))
                .ToListAsync(ct);
        }

        #endregion

        #region Private Helpers

        private async Task<ApiResponse<string>> SaveFileAsync(IFormFile file, CancellationToken ct)
        {
            try
            {
                await using var stream = file.OpenReadStream();

                var filePath = await _fileStorage.SaveFileAsyncSafe(stream, file.FileName, file.ContentType, ct);

                if (!filePath.IsSuccess)
                    return ApiResponse<string>.Fail(filePath.ErrorMessage!);
                return ApiResponse<string>.Success(filePath.FilePath!);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Failed to save file: {ex.Message}");
            }
        }

        private static Attachment CreateAttachmentEntity(
            IFormFile file,
            string filePath,
            Guid attachmentableId,
            Guid? createdById, EntityTypeEnum AttachmentableType)
        {
            return new Attachment
            {
                Id = Guid.NewGuid(),
                Name = file.FileName,
                MimeType = file.ContentType ?? "application/octet-stream",
                FilePath = filePath,
                FileSize = file.Length,
                AttachmentableId = attachmentableId,
                AttachmentableType = AttachmentableType,
                CreatedAt = DateTime.UtcNow,
                CreatedById = createdById
            };
        }

        #endregion
    }
}
