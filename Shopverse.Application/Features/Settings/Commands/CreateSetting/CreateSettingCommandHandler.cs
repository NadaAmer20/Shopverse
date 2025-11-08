using MediatR;
using Shopverse.Application.DTOs.Settings;
using Shopverse.Application.Responses;
using Shopverse.Application.Services.Attachments;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Enums;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Settings.Commands.CreateSetting
{
    public class CreateSettingCommandHandler : IRequestHandler<CreateSettingCommand, ApiResponse<SettingDto>>
    {
        private readonly IRepository<Setting> _repo;
        private readonly AttachmentService _attachmentService;

        public CreateSettingCommandHandler(
            IRepository<Setting> repo,
            AttachmentService attachmentService)
        {
            _repo = repo;
            _attachmentService = attachmentService;
        }

        public async Task<ApiResponse<SettingDto>> Handle(CreateSettingCommand request, CancellationToken ct)
        {
            string? value = null;

            if (request.Key == "Logo" || request.Key == "Favicon")
            {
                if (request.Attachment == null)
                {
                    return ApiResponse<SettingDto>.Fail("Attachment is required.");
                }

                var attachmentType = request.Key == "Logo" ? EntityTypeEnum.Logo : EntityTypeEnum.Favicon;

                var uploadResult = await _attachmentService.AddAttachmentAsync(
                    request.Attachment,
                    null,
                    null,
                    attachmentType,
                    ct
                );

                if (!uploadResult.IsSuccess)
                {
                    return ApiResponse<SettingDto>.Fail(uploadResult.Message);
                }

                value = uploadResult.Data?.FilePath;
            }
            else
            {
                value = JsonSerializer.Serialize(request.Values);
            }

            var entity = new Setting
            {
                Id = Guid.NewGuid(),
                Key = request.Key,
                Description = request.Description,
                Value = value ?? ""
            };

            await _repo.AddAsync(entity, ct);

            return ApiResponse<SettingDto>.Success(new SettingDto(entity), "Setting created successfully.");
        }

    }
}
