using System;
using System.Collections;
using System.Text;
using Utilities.Collections;

namespace QuantIsleParser
{
	public class TextArrayBuilder
	{
		public ArrayList list;
		StringBuilder sb;

		public TextArrayBuilder()
		{
			list = new ArrayList();
			sb = new StringBuilder();

		}

		public void Clear()
		{
			list.Clear();
			sb.Length = 0;
		}

		public void append(string s)
		{
			sb.Append(s);
		}

		public void append(char s)
		{
			sb.Append(s);
		}

		public void newline()
		{
			list.Add(sb.ToString());
			sb.Length = 0;
		}

		public void flush()
		{
			if (sb.Length > 0)
			{
				newline();
			}
		}

		public override string ToString()
		{
            return ArrayFormatter.ListElements(list, "\n") + "\n";
		}

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			TextArrayBuilder tab = new TextArrayBuilder();
			tab.append("Hello");
			tab.newline();
			tab.append("there");
			tab.append(" you");
			tab.append(" go");
			tab.newline();
			tab.append("final");
			tab.flush();
			Console.WriteLine(tab);
		}
#endif

        #endregion
    }
}
