using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Utilities;
using Utilities.Maintainable;

namespace Qiqqa.Common.GeneralTaskDaemonStuff
{
    public class GeneralTaskDaemon
    {
        public static readonly GeneralTaskDaemon Instance = new GeneralTaskDaemon();

        public delegate void GeneralTaskDelegate(Daemon daemon);

        class GeneralTaskItem
        {
            public string description;
            public MethodInfo method;
            public WeakReference target;
        }


        List<GeneralTaskItem> general_task_items = new List<GeneralTaskItem>();        
        
        GeneralTaskDaemon()
        {
            MaintainableManager.Instance.Register(DoMaintenance, 30000, ThreadPriority.Normal);
        }

        /// <summary>
        /// Is automatically polled while the daemon is alive
        /// </summary>
        /// <param name="daemon"></param>
        void DoMaintenance(Daemon daemon)
        {
            lock (general_task_items)
            {
                for (int i = general_task_items.Count - 1; i >= 0; --i)
                {
                    GeneralTaskItem general_task_item = general_task_items[i];

                    object target = general_task_item.target.Target;
                    if (null != target)
                    {
                        try
                        {
                            general_task_item.method.Invoke(target, new object[] { daemon });
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex, "An exception occurred in the GeneralTaskDaemon with GeneralTaskItem {0}", general_task_item.description);
                        }
                    }
                    else
                    {
                        Logging.Info("A GeneralTaskItem has been garbage collected, so throwing it away: {0}", general_task_item.description);
                        general_task_items.RemoveAt(i);
                    }
                }
            }

            daemon.Sleep(10 * 1000);
        }

        public void AddGeneralTask(GeneralTaskDelegate general_task_delegate)
        {
            lock (general_task_items)
            {
                GeneralTaskItem general_task_item = new GeneralTaskItem();
                general_task_item.description = String.Format("{0}:{1}", general_task_delegate.Target, general_task_delegate.Method.Name);
                general_task_item.method = general_task_delegate.Method;
                general_task_item.target = new WeakReference(general_task_delegate.Target);

                general_task_items.Add(general_task_item);
            }
        }

        public void RemoveGeneralTask(GeneralTaskDelegate general_task_delegate)
        {
            lock (general_task_items)
            {
                GeneralTaskItem general_task_item = new GeneralTaskItem();
                general_task_item.description = String.Format("{0}:{1}", general_task_delegate.Target, general_task_delegate.Method.Name);
                general_task_item.method = general_task_delegate.Method;
                general_task_item.target = new WeakReference(general_task_delegate.Target);

                general_task_items.Add(general_task_item);

                for (int i = 0; i < general_task_items.Count; ++i)
                {
                    if (general_task_items[i].target == general_task_delegate.Target)
                    {
                        general_task_items.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
