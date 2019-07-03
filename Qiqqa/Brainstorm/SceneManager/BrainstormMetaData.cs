using System;
using System.Reflection;
using Utilities.Misc;
using Utilities.Reflection;

namespace Qiqqa.Brainstorm.SceneManager
{
    [Serializable]
    [Obfuscation(Feature = "properties renaming")]
    public class BrainstormMetadata : DictionaryBasedObject
    {
        public string Title
        {
            get { return this["Title"] as string; }
            set { this["Title"] = value; }
        }
        
        public string Description
        {
            get { return this["Description"] as string; }
            set { this["Description"] = value; }
        }
        
        public string LastOpenLocation
        {
            get { return this["LastOpenLocation"] as string; }
            set { this["LastOpenLocation"] = value; }
        }

        public DateTime? LastSaveDate
        {
            get { return this["LastSaveDate"] as DateTime?; }
            set { this["LastSaveDate"] = value; }
        }

        [NonSerialized]
        private AugmentedBindable<BrainstormMetadata> augmented_bindable = null;
        public AugmentedBindable<BrainstormMetadata> AugmentedBindable
        {
            get
            {
                if (null == augmented_bindable)
                {
                    augmented_bindable = new AugmentedBindable<BrainstormMetadata>(this);
                }
                
                return augmented_bindable;
            }
        }
    }
}
