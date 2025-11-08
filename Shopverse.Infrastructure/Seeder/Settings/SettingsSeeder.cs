using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Infrastructure.Seeder.Settings
{
    public static class SettingsSeeder
    {
        public static async Task SeedAsync(IRepository<Setting> repo)
        {
            var defaultSettings = new List<Setting>
            {
                new Setting { Id = Guid.NewGuid(), Key = "SiteName", Description = "Site Name", Value = "My Portal" },// Done
                new Setting { Id = Guid.NewGuid(), Key = "UploadsPath", Description = "Uploads Path", Value = "uploads" }, // Don
                new Setting { Id = Guid.NewGuid(), Key = "MaxUploadSize", Description = "Maximum Upload Size in bytes", Value = "5000000" }, //Done
                new Setting { Id = Guid.NewGuid(), Key = "DefaultResultsPerPage", Description = "Default Results Per Page", Value = "10"},
                new Setting { Id = Guid.NewGuid(), Key = "Logo", Description = "Site Logo Path", Value = "" },//Done
                new Setting { Id = Guid.NewGuid(), Key = "IconMaxUploadSize", Description = "Maximum allowed file size(in bytes)", Value = "2097152" }, // 2 MB
                new Setting { Id = Guid.NewGuid(), Key = "IconAllowedExtensions", Description = "Allowed file extensions", Value = ".jpg,.jpeg,.png,.gif,.svg" },
                new Setting { Id = Guid.NewGuid(), Key = "Favicon", Description = "Site Favicon Path", Value = "" },//Done
                new Setting { Id = Guid.NewGuid(), Key = "Smtp_Host", Description = "SMTP Server Host", Value = "smtp.gmail.com" },//Done
                new Setting { Id = Guid.NewGuid(), Key = "Smtp_Port", Description = "SMTP Server Port", Value = "587" },//Done
                new Setting { Id = Guid.NewGuid(), Key = "Smtp_Username", Description = "SMTP Username", Value = "timezone706@gmail.com"},//Done
                new Setting { Id = Guid.NewGuid(), Key = "Smtp_Password", Description = "SMTP Password", Value = "fzjr aifk inpx wrau"},//Done
                new Setting { Id = Guid.NewGuid(), Key = "Smtp_From", Description = "SMTP From Email", Value = "no-reply@pnustudentportal.com" },//Done
                new Setting { Id = Guid.NewGuid(), Key = "Smtp_EnableSsl", Description = "SMTP Enable SSL", Value = "true"  },//Done
                new Setting { Id = Guid.NewGuid(), Key = "FileAllowedExtensions", Description = "Allowed file extensions", Value =".jpg,.jpeg,.png,.gif,.svg,.bmp,.tiff,.webp,.mp4,.mov,.avi,.pdf,.docx,.xlsx"  },


            };

            foreach (var s in defaultSettings)
            {
                var exists = repo.GetQueryable().Any(x => x.Key == s.Key);
                if (!exists) await repo.AddAsync(s);
            }
        }
    }
}
