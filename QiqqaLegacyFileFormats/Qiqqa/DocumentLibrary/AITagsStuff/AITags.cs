using System;
using System.Collections.Generic;
using ProtoBuf;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.DocumentLibrary.AITagsStuff
{
    [Serializable]
    [ProtoContract]
    public class AITags
    {
        [ProtoMember(1)]
        private DateTime timestamp_generated;
        [ProtoMember(2)]
        private MultiMapSet<string, string> ai_tags_with_documents; // tag -> documents
        [ProtoMember(3)]
        private MultiMapSet<string, string> ai_documents_with_tags; // document -> tags

        public AITags()
        {
            timestamp_generated = DateTime.UtcNow;
            ai_tags_with_documents = new MultiMapSet<string, string>();
            ai_documents_with_tags = new MultiMapSet<string, string>();
        }
    }
}
