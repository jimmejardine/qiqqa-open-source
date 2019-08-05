using System;

namespace Utilities.DateTimeTools
{
    /// <summary>
    /// Use as IDisposable to automatically log time elapsed and description on disposal.
    /// </summary>
	public class StopWatch : IDisposable
	{
		static readonly DateTime NULL_TIME = DateTime.MinValue;

		DateTime start_timestamp = NULL_TIME;
        DateTime stop_timestamp = NULL_TIME;

        public string Description { get; set; }

		public StopWatch() : this("???")
		{
		}

        public StopWatch(string description, params object[] args)
        {
            Description = string.Format(description, args);
            Start();
        }

	    public void Start()
		{
			start_timestamp = DateTime.Now;
			stop_timestamp = NULL_TIME;
		}

		public void Stop()
		{
			stop_timestamp = DateTime.Now;
		}

		public double ElapsedSeconds()
		{
			if (NULL_TIME == stop_timestamp)
			{
				TimeSpan span = DateTime.Now.Subtract(start_timestamp);
				return span.TotalSeconds;
			}
			else
			{
				TimeSpan span = stop_timestamp.Subtract(start_timestamp);
				return span.TotalSeconds;
				
			}
		}

        public double ElapsedMilliseconds()
        {
            if (NULL_TIME == stop_timestamp)
            {
                TimeSpan span = DateTime.Now.Subtract(start_timestamp);
                return span.TotalMilliseconds;
            }
            else
            {
                TimeSpan span = stop_timestamp.Subtract(start_timestamp);
                return span.TotalMilliseconds;

            }
        }
        
        public override string ToString()
		{
			if (NULL_TIME == stop_timestamp)
			{
				return "Started " + start_timestamp + ", elapsed " + ElapsedSeconds() + " seconds";
			}
			else
			{
				return "Started " + start_timestamp + ", stopped " + stop_timestamp + ", elapsed " + ElapsedSeconds() + " seconds";
			}
		}

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            Stop();
            Logging.Debug("{0} in {1}ms", Description, ElapsedMilliseconds());
            disposed = true;

            //GC.SuppressFinalize(this);
        }
	}
}
