using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Settings.Commands.UpdateSetting
{
    public class UpdateSettingCommandValidator : AbstractValidator<UpdateSettingCommand>
    {
        private readonly IRepository<Setting> _settingRepo;

        private static readonly List<string> ImageKeys = new() { "Logo", "Favicon" };
        private static readonly List<string> NumericKeys = new() { "MaxUploadSize", "DefaultResultsPerPage", "Smtp_Port" };
        private static readonly List<string> BoolKeys = new() { "Smtp_EnableSsl" };
        private static readonly List<string> EmailKeys = new() { "Smtp_Username", "Smtp_From" };
        private static readonly List<string> TextKeys = new() { "SiteName", "UploadsPath", "Smtp_Host" };

        public UpdateSettingCommandValidator(IRepository<Setting> settingRepo)
        {
            _settingRepo = settingRepo;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Setting Id is required.");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");

            When(x => !string.IsNullOrEmpty(x.Key), () =>
            {
                When(x => ImageKeys.Contains(x.Key), () =>
                {
                    RuleFor(x => x.Attachment)
                        .NotNull().WithMessage("Attachment is required.")
                        .Must(BeValidImage).WithMessage("Invalid image format.");
                });

                When(x => NumericKeys.Contains(x.Key), () =>
                {
                    RuleFor(x => x.Values)
                        .NotNull().WithMessage("Values are required.")
                        .Must(BeValidNumber).WithMessage("Invalid number format.");
                });

                When(x => BoolKeys.Contains(x.Key), () =>
                {
                    RuleFor(x => x.Values)
                        .NotNull().WithMessage("Values are required.")
                        .Must(BeValidBool).WithMessage("Invalid boolean value.");
                });

                When(x => EmailKeys.Contains(x.Key), () =>
                {
                    RuleFor(x => x.Values)
                        .NotNull().WithMessage("Email value is required.")
                        .Must(BeValidEmail).WithMessage("Invalid email format.");
                });

                When(x => TextKeys.Contains(x.Key), () =>
                {
                    RuleFor(x => x.Values)
                        .NotNull().WithMessage("Values are required.")
                        .Must(v => v.All(value => !string.IsNullOrWhiteSpace(value)))
                        .WithMessage("Text values cannot be empty.");
                });
            });
        }

        private static bool BeValidImage(IFormFile? file)
        {
            if (file == null) return false;
            var allowed = new[] { ".png", ".jpg", ".jpeg", ".ico" };
            var ext = System.IO.Path.GetExtension(file.FileName).ToLower();
            return allowed.Contains(ext);
        }

        private static bool BeValidNumber(List<string>? values) => values != null && values.All(v => long.TryParse(v, out var num) && num >= 0);

        private static bool BeValidBool(List<string>? values) => values != null && values.All(v => v.Equals("true", System.StringComparison.OrdinalIgnoreCase) || v.Equals("false", System.StringComparison.OrdinalIgnoreCase));

        private static bool BeValidEmail(List<string>? values) => values != null && values.All(v => v.Contains("@") && v.Contains("."));
    }
}
