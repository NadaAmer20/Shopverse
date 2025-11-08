using Shopverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.DTOs.Attachments
{
    public class AttachmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Identifier { get; set; } = "Attachment";
        public string AttachmentableType { get; set; }

        public AttachmentDto() { }

        public AttachmentDto(Attachment a)
        {
            Id = a.Id;
            Name = a.Name;
            MimeType = a.MimeType;
            FilePath = a.FilePath;
            FileSize = a.FileSize;
            AttachmentableType = a.AttachmentableType.ToString();
        }
    }
}
