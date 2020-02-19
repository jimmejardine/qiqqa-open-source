using System;
using System.Collections;

namespace QiqqaLegacyFileFormats          // namespace Utilities.Collections
{
    public class TwoKeyStoredDictionary : TwoKeyDictionary
    {
        private string base_filename;

        public TwoKeyStoredDictionary(string abase_filename)
        {
            base_filename = abase_filename;
        }

        // ---

        public override void Clear()
        {
            // We may need to go to all the effort of deleting the files...
            throw new Exception("Method not implemented");
        }

        // ---

        public override bool Contains(object key1)
        {
            if (trunk.Contains(key1))
            {
                return true;
            }

            fetchBranchFromDisk(key1);
            return trunk.Contains(key1);
        }

        public override Hashtable Get(object key1)
        {
            // If it is already in the tree, then we use the in-memory object
            if (Contains(key1))
            {
                return (Hashtable)trunk[key1];
            }

            fetchBranchFromDisk(key1);
            return (Hashtable)trunk[key1];
        }

        // ---

        public void commit()
        {
            foreach (object key1 in trunk.Keys)
            {
                saveBranchToDisk(key1);
            }
        }

        // ---

        private void fetchBranchFromDisk(object key1)
        {
            try
            {
                Hashtable branch = (Hashtable)SerializeFile.Load(base_filename + "." + key1.ToString());
                trunk[key1] = branch;
            }

            catch (Exception e)
            {
                Console.WriteLine("Error loading branch {0}: {1}", key1, e.Message);
            }
        }

        private void saveBranchToDisk(object key1)
        {
            try
            {
                SerializeFile.Save(base_filename + "." + key1.ToString(), trunk[key1]);
            }

            catch (Exception e)
            {
                Console.WriteLine("Error saving branch {0}: {1}", key1, e.Message);
            }
        }
    }
}
