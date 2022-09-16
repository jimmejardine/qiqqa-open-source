using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Qiqqa.Common.GUI;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Common
{
    /// <summary>
    /// Interaction logic for ActivityMonitor.xaml
    /// </summary>
    public partial class ActivityMonitor : StandardWindow
    {
        public ActivityMonitor()
        {
            //Theme.Initialize(); -- already done in StandardWindow base class

            InitializeComponent();

            Initialized += ActivityMonitor_Initialized;
            Closing += ActivityMonitor_Closing;
            Closed += ActivityMonitor_Closed;


            //  DispatcherTimer setup
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += WeakEventHandler2.Wrap(dispatcherTimer_Tick, (eh) =>
            {
                dispatcherTimer.Tick -= eh;
            });
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Constants.UI_REFRESH_POLLING_INTERVAL);
            dispatcherTimer.Start();
        }

        //private long library_change_marker_tick = 0;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
        }

        private void ActivityMonitor_Initialized(object sender, EventArgs e)
        {
            ActivityMonitorCore.ActivityMonitorWindow = this;
        }

        private void ActivityMonitor_Closed(object sender, EventArgs e)
        {
        }

        private void ActivityMonitor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ActivityMonitorCore.ActivityMonitorWindow = null;
        }
    }

    // ---------------------------------------------------------------------------------------

    public struct UIResponseTiming
    {
        public long ui_invoke_duration;
        public long ui_invoke_bg_duration;

        public long ui_roundtrip_duration;
        public long ui_roundtrip_bg_duration;

        public long threadpool_start_delay;
        public long threadpool_start_delay_bg;
    }

    public class ActivityDataRecord
    {
        public UIResponseTiming UIResponsiveness;
        public MemoryStatus memory;
    }

    public delegate void ActivityDataRecordProcessor(ActivityDataRecord rec, int idx);
    public delegate void ActivityCurrentDataRecordProcessor(ActivityDataRecord rec, ActivityDataRecord old);

    public class ActivityHistory
    {
        public const int ACTIVITY_HISTORY_DEPTH = 120;

        // a cyclic buffer:
        internal int current_index = 0;
        internal ActivityDataRecord[] collected_data_history = new ActivityDataRecord[ACTIVITY_HISTORY_DEPTH];
        internal object data_lock = new object();
        internal object index_lock = new object();

        public ActivityHistory()
        {
            for (int i = 0; i < ACTIVITY_HISTORY_DEPTH; i++)
            {
                collected_data_history[i] = new ActivityDataRecord();
            }
        }

        public void ProcessCurrentRecord(bool shift, ActivityCurrentDataRecordProcessor processor)
        {
            lock (index_lock)
            {
                int old_index = current_index;
                if (shift)
                {
                    // move on to the NEXT record, i.e. file the current record as historical from now on:
                    current_index = (current_index + 1) % ACTIVITY_HISTORY_DEPTH;
                }

                lock (data_lock)
                {
                    processor(collected_data_history[current_index], collected_data_history[old_index]);
                }
            }
        }

        // walks the history from young to old, i.e. in REVERSE TIMELINE ORDER!
        public void ProcessHistory(ActivityDataRecordProcessor processor, int oldest_offset = ACTIVITY_HISTORY_DEPTH - 1, int newest_offset = 0)
        {
            // the key to locking here is to prevent any other thread to MOVE the index
            // OR modify the latest record. The older records are immutable anyway.
            if (oldest_offset < 0 || oldest_offset >= ACTIVITY_HISTORY_DEPTH)
            {
                throw new Exception($"oldest:offset {oldest_offset} out of range");
            }
            if (newest_offset < 0 || newest_offset >= ACTIVITY_HISTORY_DEPTH)
            {
                throw new Exception($"newest:offset {newest_offset} out of range");
            }
            if (newest_offset > oldest_offset)
            {
                throw new Exception($"newest:offset {newest_offset} is OLDER than oldest:offset {oldest_offset}");
            }

            int user_index = 0;
            // To ensure the processor gets to mess around with the most recent history record without collisions,
            // we LOCK the entire record process for this one!
            lock (index_lock)
            {
                if (newest_offset == 0)
                {
                    lock (data_lock)
                    {
                        processor(collected_data_history[current_index], user_index);
                    }
                    user_index++;
                    newest_offset++;
                }

                // all the while keeping the INDEX locked, we now feed the processor the remaining history records:
                for (; newest_offset <= oldest_offset; newest_offset++, user_index++)
                {
                    int i = (current_index + ACTIVITY_HISTORY_DEPTH - newest_offset) % ACTIVITY_HISTORY_DEPTH;
                    processor(collected_data_history[i], user_index);
                }
            }
        }
    }

    public class ActivityMonitorCore
    {
        static ActivityMonitorCore()
        {
        }

        internal static WeakReference<ActivityMonitor> activity_window = null;
        internal static object activity_window_lock = new object();

        public static ActivityMonitor ActivityMonitorWindow
        {
            get
            {
                lock (activity_window_lock)
                {
                    if (activity_window != null && activity_window.TryGetTarget(out ActivityMonitor rv) && rv != null)
                    {
                        return rv;
                    }
                    return null;
                }
            }
            set
            {
                lock (activity_window_lock)
                {
                    if (value != null)
                    {
                        activity_window = new WeakReference<ActivityMonitor>(value);
                    }
                    else
                    {
                        activity_window = null;
                    }
                }
            }
        }

        private struct UIResponseTimingHousekeeeping
        {
            internal int running;
            internal Stopwatch clkNormal;
            internal Stopwatch clkBackground;
            internal Stopwatch clk2Test;
        }

        internal const int ACTIVITY_HISTORY_DEPTH = 120;

        internal static ActivityHistory collected_data = new ActivityHistory();
        private static UIResponseTimingHousekeeeping resp_housekeeping = new UIResponseTimingHousekeeeping();

        public static event EventHandler<EventArgs> OnTick = null;

        // NOTE:
        // We could've used the DispatchTimer for this one (https://social.msdn.microsoft.com/Forums/vstudio/en-US/e7b66990-89c3-4958-b4e7-1d2ad1d4dd74/wpf-multitasking-along-with-a-timer-and-background-worker)
        // as the results as largely important only for the UI, but I've decided to
        // run this bugger in
        public static void BackgroundTask()
        {
            // We FIRST exec the UI update routine, because the UI timing parts of the CURRENT history record
            // will only now have been updated by the additional measurement background tasks.
            // If we would to this OnTick UI update work LAST, we would always lag one 'tick' (~ 2 seconds)
            // behind on timings, which would then show a flat leading part in any chart.
            OnTick?.Invoke(null, null);

            // history record based graphs, etc. *too*!
            // GC memory pressure?
            MemoryStatus mstat = ComputerStatistics.GetMemoryStatus();
            bool are_timing_tests_running = false;

            // SHIFT the cursor one record forward and fill the new record as best as possible.
            // This means we have a 'fixed sample frequency' in the history, as set up in the
            // BackgroundWorkerDaemon.
            collected_data.ProcessCurrentRecord(shift: true, (rec, old) =>
            {
                rec.memory = mstat;
                rec.UIResponsiveness = old.UIResponsiveness;
                are_timing_tests_running = (resp_housekeeping.running > 0);

                // the easiest way to ensure the timing values stay until updated, is to copy
                // the now-old values:
            });

            // measure UI responsiveness? Only when we won't collide with still running tests!
            if (!are_timing_tests_running)
            {
                lock (collected_data.data_lock)
                {
                    resp_housekeeping.running = 2;

                    resp_housekeeping.clk2Test = Stopwatch.StartNew();
                }
                SafeThreadPool.QueueUserWorkItem(() => MeasureUIResponsivenessNormal());
                SafeThreadPool.QueueUserWorkItem(() => MeasureUIResponsivenessBackground());
            }
        }

        internal static void MeasureUIResponsivenessNormal()
        {
            // measure UI responsiveness
            collected_data.ProcessCurrentRecord(shift: false, (rec, _) =>
            {
                rec.UIResponsiveness.threadpool_start_delay = resp_housekeeping.clk2Test.ElapsedTicks;
                resp_housekeeping.clkNormal = Stopwatch.StartNew();
            });

            // Relinquish control to the UI thread temporarily to help check UI responsiveness
            WPFDoEvents.InvokeInUIThread(() =>
            {
                // do nothing. Just measure...

                collected_data.ProcessCurrentRecord(shift: false, (rec, _) =>
                {
                    rec.UIResponsiveness.ui_invoke_duration = resp_housekeeping.clkNormal.ElapsedTicks;
                });
            });

            collected_data.ProcessCurrentRecord(shift: false, (rec, _) =>
            {
                rec.UIResponsiveness.ui_roundtrip_duration = resp_housekeeping.clkNormal.ElapsedTicks;

                resp_housekeeping.running--;
            });
        }

        internal static void MeasureUIResponsivenessBackground()
        {
            // measure UI responsiveness
            collected_data.ProcessCurrentRecord(shift: false, (rec, _) =>
            {
                rec.UIResponsiveness.threadpool_start_delay_bg = resp_housekeeping.clk2Test.ElapsedTicks;
                resp_housekeeping.clkBackground = Stopwatch.StartNew();
            });

            // Relinquish control to the UI thread temporarily to help check UI responsiveness
            WPFDoEvents.InvokeInUIThread(() =>
            {
                // do nothing. Just measure...

                collected_data.ProcessCurrentRecord(shift: false, (rec, _) =>
                {
                    rec.UIResponsiveness.ui_invoke_bg_duration = resp_housekeeping.clkBackground.ElapsedTicks;
                });
            });

            collected_data.ProcessCurrentRecord(shift: false, (rec, _) =>
            {
                rec.UIResponsiveness.ui_roundtrip_bg_duration = resp_housekeeping.clkBackground.ElapsedTicks;

                resp_housekeeping.running--;
            });
        }
    }
}
