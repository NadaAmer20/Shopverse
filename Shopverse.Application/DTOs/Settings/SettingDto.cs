using System.Text.Json;

namespace Shopverse.Application.DTOs.Settings
{
    public class SettingDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new();
        public string? RawValue { get; set; }
        public DateTime CreatedAt { get; set; }

        public SettingDto() { }

        public SettingDto(Domain.Entities.Setting setting)
        {
            Id = setting.Id;
            Key = setting.Key;
            Description = setting.Description;
            CreatedAt = setting.CreatedAt;
            RawValue = setting.Value;

            if (!string.IsNullOrWhiteSpace(setting.Value))
            {
                try
                {
                    if (setting.Value.TrimStart().StartsWith("["))
                    {
                        Values = JsonSerializer.Deserialize<List<string>>(setting.Value) ?? new List<string>();
                    }
                    else if (setting.Value.TrimStart().StartsWith("{"))
                    {
                        Values = new List<string> { setting.Value };
                    }
                    else
                    {
                        Values = new List<string> { setting.Value };
                    }
                }
                catch
                {
                    Values = new List<string> { setting.Value };
                }
            }
        }
    }
}
