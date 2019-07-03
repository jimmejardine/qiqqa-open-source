using System;
using System.Text;

namespace Utilities.Collections
{
	/// <summary>
	/// Summary description for Splitter.
	/// </summary>
	public class Splitter
	{
		public static String combineStringAtChar(String[] source, char split_char)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < source.Length-1; ++i)
			{
				sb.Append(source[i]);
				sb.Append(split_char);
			}
			sb.Append(source[source.Length-1]);

			return sb.ToString();
		}


		public static String[] splitStringAtChar(String source, char split_char)
		{
			int splitcount = 0;

			// Count the number of split characters
			int source_length = source.Length;
			for (int i = 0; i < source_length; ++i)
			{
				if (split_char == source[i]) ++splitcount;
			}

			// Add in a terminating split char to simplify the algorithm
			String search = source + split_char;
			int search_length = search.Length;

			// Extract each of the strings
			int splitpos = 0;
			int last_extractpos = 0;
			String[] result = new String[splitcount+1];
			for (int i = 0; i < search_length; ++i)
			{
				if (split_char == search[i])
				{
					result[splitpos] = search.Substring(last_extractpos, i-last_extractpos);
					last_extractpos = i+1;
					++splitpos;
				}
			}

			return result;
		}

		public static String[] splitStringAtFirstChar(String source, char split_char)
		{
			// Always have two values, even if they end up being ""
			String[] result = new String[2];
	
			// Extract each of the strings
			int splitpos = source.IndexOf(split_char);
			if (-1 == splitpos)
			{
				result[0] = source;
				result[1] = "";
			}
			else
			{
				result[0] = source.Substring(0, splitpos);
				if (splitpos+1 < source.Length)
				{
					result[1] = source.Substring(splitpos+1);
				}
				else
				{
					result[1] = "";
				}
			}

			return result;
		}
	}
}
