﻿using Microsoft.Extensions.Logging;

namespace Orchard.ContentManagement.Handlers
{
    public class ContentItemMetadataContext
    {
        public ContentItem ContentItem { get; set; }
        public ContentItemMetadata Metadata { get; set; }
    }
}
