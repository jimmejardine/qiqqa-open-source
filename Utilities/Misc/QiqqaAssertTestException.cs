using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Misc
{
    public class QiqqaAssertTestException : ApplicationException
    {
        public QiqqaAssertTestException(string message = null)
            : base((String.IsNullOrEmpty(message) ? "Assertion Failure" : message) + "\n### Report this issue at the GitHub Qiqqa project issue page https://github.com/jimmejardine/qiqqa-open-source/issues and please do provide the log file which contains this error report and accompanying stacktrace.")
        {
        }

        public QiqqaAssertTestException(string message, Exception inner_exception)
            : base((String.IsNullOrEmpty(message) ? "Assertion Failure" : message) + "\n### Report this issue at the GitHub Qiqqa project issue page https://github.com/jimmejardine/qiqqa-open-source/issues and please do provide the log file which contains this error report and accompanying stacktrace.", inner_exception)
        {
        }
    }

    public static class ASSERT
    {
        public static void Test(bool test, string message = null)
        {
            if (!test)
            {
                throw new QiqqaAssertTestException(message);
            }
        }
    }
}
