using Microsoft.AspNetCore.Http;
using Shopverse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.DTOs.Attachments
{
    public class AttachmentFormModel
    {
        public EntityTypeEnum AttachmentableType { get; set; }
        public IFormFile File { get; set; } = default!;
    }

}
