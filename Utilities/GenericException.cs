using System;

namespace Utilities
{
	public class GenericException : Exception
	{
        public GenericException(string message)
            : base(message)
		{		
		}

        public GenericException(string message, params object[] args)
            : base(String.Format(message, args))
        {
        }

        public GenericException(Exception ex, string message, params object[] args)
            : base(String.Format(message, args), ex)
        {
        }
	}
}
