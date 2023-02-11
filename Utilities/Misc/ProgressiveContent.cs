using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Misc
{
    public enum ProgressiveContentState
    {
        Empty = 0,    // the content has been cleared/cleaned up

        Start,        // We're just beginning. Nothing has been set up yet.

        // Up to 4 stages of "pending", i.e. progressive refinement of the content:
        Pending_1,
        Pending_2,
        Pending_3,
        Pending_4,

        Final,       // We have arrived at the final state. Nothing to do any more!
    };

    class ProgressiveContent<T>
    {
        public T content;
        public ProgressiveContentState state;

        public ProgressiveContent(T initial_value)
        {
            content = initial_value;
            state = ProgressiveContentState.Start;
        }
    }
}

