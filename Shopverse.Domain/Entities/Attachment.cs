using Shopverse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Domain.Entities
{
    public class Attachment : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public Guid AttachmentableId { get; set; }
        public EntityTypeEnum AttachmentableType { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }

    }

}
