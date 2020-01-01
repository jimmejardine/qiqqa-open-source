using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        [Conditional("DEBUG")]
        public static void Test(bool test, string message = null, bool dont_throw = false)
        {
            if (!test)
            {
                Exception ex = new QiqqaAssertTestException(message);
                Logging.Error(ex, "ASSERTION FAILURE!");

                // IFF This assertion check is important, but *not* severe enough to barf a hairball when it fails: dont_throw=true, hence
                // we then *do* report the assertion failure via Logging.Error() but we *do not* follow up with throwing an exception.
                if (!dont_throw)
                {
                    throw ex;
                }
            }
        }
    }
}
