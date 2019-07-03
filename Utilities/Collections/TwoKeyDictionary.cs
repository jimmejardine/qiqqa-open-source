using System;
using System.Collections;

namespace Utilities.Collections
{
	[Serializable]
	public class TwoKeyDictionary
	{
		protected Hashtable trunk = new Hashtable();


		public TwoKeyDictionary()
		{
		}

		// ---

		public virtual void Clear()
		{
			trunk.Clear();
		}

		// ---

		public virtual void Set(object key1, Hashtable branch)
		{
			trunk[key1] = branch;
		}

		public virtual bool Contains(object key1)
		{
			return trunk.Contains(key1);
		}

		public virtual Hashtable Get(object key1)
		{
			// If it is already in the tree, then we use the in-memory object
			if (Contains(key1))
			{
				return (Hashtable) trunk[key1];
			}

			// If it is not in the tree, return null
			return null;
		}

		// ---

		public void Set(object key1, object key2, object data)
		{
			Hashtable branch = Get(key1);

			// If the branch does not exist, make one
			if (null == branch)
			{
				branch = new Hashtable();
				Set(key1, branch);
			}

			// Then store the data in the branch
			branch[key2] = data;
		}


		public object Get(object key1, object key2)
		{
			Hashtable branch = Get(key1);

			// If the branch does not even exist, return null
			if (null == branch)
			{
				return null;
			}

			// Return the contents of the branch (possibly null)
			return branch[key2];
		}


		public bool Contains(object key1, object key2)
		{
			Hashtable branch = Get(key1);

			// If the branch does not even exist, return null
			if (null == branch)
			{
				return false;
			}

			// Return the contents of the branch (possibly null)
			return branch.Contains(key2);
		}

		// ---
		
		public Hashtable this[object key1]
		{
			get
			{
				return Get(key1);
			}

			set
			{
				Set(key1, (Hashtable) value);
			}
		}

		public object this[object key1, object key2]
		{
			get
			{
				return Get(key1, key2);
			}

			set
			{
				Set(key1, key2, value);
			}
		}
	}
}
