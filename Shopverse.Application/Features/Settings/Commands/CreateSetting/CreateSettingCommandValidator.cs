using FluentValidation;
using Microsoft.AspNetCore.Http;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Shopverse.Application.Features.Settings.Commands.CreateSetting
{
    public class CreateSettingCommandValidator : AbstractValidator<CreateSettingCommand>
    {
        private readonly IRepository<Setting> _settingRepo;

        private static readonly List<string> ImageKeys = new() { "Logo", "Favicon" };
        private static readonly List<string> NumericKeys = new() { "MaxUploadSize", "DefaultResultsPerPage", "Smtp_Port" };
        private static readonly List<string> BoolKeys = new() { "Smtp_EnableSsl" };
        private static readonly List<string> EmailKeys = new() { "Smtp_Username", "Smtp_From" };
        private static readonly List<string> TextKeys = new() { "SiteName", "UploadsPath", "Smtp_Host" };

        public CreateSettingCommandValidator(IRepository<Setting> settingRepo)
        {
            _settingRepo = settingRepo;

            RuleFor(x => x.Key)
                .NotEmpty().WithMessage("Key is required.")
                .MaximumLength(100).WithMessage("Key cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");

            When(x => !ImageKeys.Contains(x.Key), () =>
            {
                RuleFor(x => x.Values)
                    .NotNull().WithMessage("Values are required.")
                    .Must(v => v.Any()).WithMessage("Values cannot be empty.");
            });

            When(x => ImageKeys.Contains(x.Key), () =>
            {
                RuleFor(x => x.Attachment)
                    .NotNull().WithMessage("Attachment is required.")
                    .Must(BeValidImage).WithMessage("Invalid image format.");
            });

            When(x => NumericKeys.Contains(x.Key), () =>
            {
                RuleFor(x => x.Values)
                    .Must(BeValidNumber).WithMessage("Invalid number format.");
            });

            When(x => BoolKeys.Contains(x.Key), () =>
            {
                RuleFor(x => x.Values)
                    .Must(BeValidBool).WithMessage("Invalid boolean value.");
            });

            When(x => EmailKeys.Contains(x.Key), () =>
            {
                RuleFor(x => x.Values.FirstOrDefault())
                    .NotEmpty().EmailAddress().WithMessage("Invalid email address.");
            });

            When(x => x.Key == "SiteName", () =>
            {
                RuleFor(x => x.Values.FirstOrDefault())
                    .NotEmpty().WithMessage("SiteName is required.")
                    .MaximumLength(100).WithMessage("SiteName cannot exceed 100 characters.");
            });
        }

        private static bool BeValidImage(IFormFile? file)
        {
            if (file == null) return false;
            var allowed = new[] { ".png", ".jpg", ".jpeg", ".ico" };
            var ext = System.IO.Path.GetExtension(file.FileName).ToLower();
            return allowed.Contains(ext);
        }

        private static bool BeValidNumber(List<string>? values)
        {
            if (values == null || !values.Any()) return false;
            return values.All(v => long.TryParse(v, out var num) && num >= 0);
        }

        private static bool BeValidBool(List<string>? values)
        {
            if (values == null || !values.Any()) return false;
            return values.All(v => v.ToLower() == "true" || v.ToLower() == "false");
        }
    }
}
