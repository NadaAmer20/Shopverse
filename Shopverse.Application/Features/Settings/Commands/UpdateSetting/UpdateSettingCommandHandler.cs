using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopverse.Application.DTOs.Settings;
using Shopverse.Application.Features.Settings.Commands.UpdateSetting;
using Shopverse.Application.Responses;
using Shopverse.Application.Services.Attachments;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Enums;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Settings.Commands.UpdateSetting
{
    public class UpdateSettingCommandHandler : IRequestHandler<UpdateSettingCommand, ApiResponse<SettingDto>>
    {
        private readonly IRepository<Setting> _settingRepo;
        private readonly IRepository<Attachment> _attachmentRepo;
        private readonly AttachmentService _attachmentService;

        public UpdateSettingCommandHandler(
            IRepository<Setting> settingRepo,
            IRepository<Attachment> attachmentRepo,
            AttachmentService attachmentService)
        {
            _settingRepo = settingRepo;
            _attachmentRepo = attachmentRepo;
            _attachmentService = attachmentService;
        }

        public async Task<ApiResponse<SettingDto>> Handle(UpdateSettingCommand request, CancellationToken ct)
        {
            var entity = await _settingRepo.GetByIdAsync(request.Id);
            if (entity == null)
                return ApiResponse<SettingDto>.Fail("Setting not found.");

            if (!string.IsNullOrWhiteSpace(request.Description))
                entity.Description = request.Description;

            if (IsAttachmentSetting(entity.Key))
            {
                if (request.Attachment != null)
                {
                    await RemoveOldAttachmentAsync(entity.Key, ct);

                    EntityTypeEnum attachmentType = entity.Key switch
                    {
                        "Logo" => EntityTypeEnum.Logo,
                        "Favicon" => EntityTypeEnum.Favicon,
                        _ => EntityTypeEnum.Setting
                    };

                    var uploadResult = await _attachmentService.AddAttachmentAsync(
                        request.Attachment,
                        null,
                        null,
                        attachmentType,
                        ct
                    );

                    if (!uploadResult.IsSuccess)
                        return ApiResponse<SettingDto>.Fail(uploadResult.Message);

                    entity.Value = uploadResult.Data?.FilePath ?? "";
                }
            }
            else if (request.Values != null && request.Values.Any())
            {
                entity.Value = JsonSerializer.Serialize(request.Values);
            }

            request.Key = entity.Key;
            await _settingRepo.UpdateAsync(entity, ct);

            return ApiResponse<SettingDto>.Success(new SettingDto(entity), "Setting updated successfully.");
        }

        private static bool IsAttachmentSetting(string key) =>
            key.Equals("Logo", StringComparison.OrdinalIgnoreCase) || key.Equals("Favicon", StringComparison.OrdinalIgnoreCase);

        private async Task RemoveOldAttachmentAsync(string key, CancellationToken ct)
        {
            var oldAttachments = await _attachmentRepo.GetQueryable()
                .ToListAsync(ct);

            if (oldAttachments.Any())
            {
                var ids = oldAttachments.Select(a => a.Id).ToList();
                await _attachmentService.RemoveAttachmentsAsync(ids, ct);
            }
        }
    }
}
