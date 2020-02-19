using System;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.SceneManager
{
    [Serializable]
    public class BrainstormMetadata : DictionaryBasedObject
    {
        public string Title
        {
            get => this["Title"] as string;
            set => this["Title"] = value;
        }

        public string Description
        {
            get => this["Description"] as string;
            set => this["Description"] = value;
        }

        public string LastOpenLocation
        {
            get => this["LastOpenLocation"] as string;
            set => this["LastOpenLocation"] = value;
        }

        public DateTime? LastSaveDate
        {
            get => this["LastSaveDate"] as DateTime?;
            set => this["LastSaveDate"] = value;
        }
    }
}
