using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Misc
{
    public class ProgressLogger
    {
        int count;
        int total_count;

        public ProgressLogger(int total_count)
        {
            this.count = 0;
            this.total_count = total_count;
        }

        public void Tick()
        {
            ++count;
            if (0 == count % 10) Logging.Info("{0} / {1} = {2}%", count, total_count, 100.0 * count / total_count);
        }
    }
}
