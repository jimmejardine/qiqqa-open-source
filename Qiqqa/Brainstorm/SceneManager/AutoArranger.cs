using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Qiqqa.Brainstorm.Connectors;
using Qiqqa.Brainstorm.Nodes;
using Utilities;
using Utilities.Mathematics.Geometry;

namespace Qiqqa.Brainstorm.SceneManager
{
    public class AutoArranger
    {
        private object thread_lock = new object();
        private Daemon active_thread;

        // WARNING:
        //
        // SceneRenderingControl is not thread-safe, nor is this code. We'll survive with a few glitches due to that
        // as this is only about rendering a network view, not about accessing critical data. Hence we forego the
        // effort to make this truly thread-safe: it's not worth it.
        //
        private SceneRenderingControl scene_rendering_control;

        public AutoArranger(SceneRenderingControl scene_rendering_control)
        {
            this.scene_rendering_control = scene_rendering_control;
        }

        public void Enabled(bool enabled)
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (thread_lock)
            {
                //l1_clk.LockPerfTimerStop();
                if (enabled)
                {
                    // USER WANTS TO ENABLE
                    if (null == active_thread)
                    {
                        active_thread = new Daemon(daemon_name: "AutoArranger:Background");
                        //active_thread.IsBackground = true;
                        //active_thread.Priority = ThreadPriority.Lowest;
                        active_thread.Start(BackgroundThread, active_thread);
                    }
                }
                else
                {
                    // USER WANTS TO DISABLE
                    active_thread = null;
                }
            }
        }

        private void BackgroundThread(object arg)
        {
            Daemon daemon = (Daemon)arg;
            Logging.Debug特("AutoArranger Thread {0} has started", daemon.ManagedThreadId);

            while (!Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (thread_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    if (daemon != active_thread)
                    {
                        break;
                    }
                }

                DoLayout();

                daemon.Sleep(30);
            }

            Logging.Debug特("AutoArranger Thread {0} has exited", daemon.ManagedThreadId);
        }

        private long cache_scene_changed_marker = -1;
        private List<NodeControl> cache_node_controls = new List<NodeControl>();
        private List<ConnectorControl> cache_connector_controls = new List<ConnectorControl>();

        private void DoLayout()
        {
            int SPEED = 1;

            // If the nodes and connectors have changed, recache them!
            if (cache_scene_changed_marker != scene_rendering_control.SceneChangedMarker)
            {
                Logging.Info("Scene has changed, so autolayout is recaching.");
                cache_scene_changed_marker = scene_rendering_control.SceneChangedMarker;
                cache_node_controls = new List<NodeControl>(scene_rendering_control.NodeControls);
                cache_connector_controls = new List<ConnectorControl>(scene_rendering_control.ConnectorControlManager.ConnectorControls);
            }

            // We reuse this so that it is memory allocation time efficient
            NodesVector vector = new NodesVector();

            // Also note that Utilities codebase had ATTRACTION *before* REPULSION.
            // Haven't looked at the precise code, but wouldn't be surprised if this is
            // very similar to the D3 force anneal code (D3.js) anyway. There aren't that
            // many ways to stabilize a (large) graph in 2D.
            //
            // See also https://github.com/jimmejardine/qiqqa-open-source/issues/26

            // Perform the repulsion
            if (true)
            {
                int MAX_NODES = cache_node_controls.Count;
                for (int i = 0; i < MAX_NODES; ++i)
                {
                    NodeControlSceneData nodeI = cache_node_controls[i].NodeControlSceneData;
                    if (nodeI.Deleted)
                    {
                        continue;
                    }

                    for (int j = i + 1; j < MAX_NODES; ++j)
                    {
                        NodeControlSceneData nodeJ = cache_node_controls[j].NodeControlSceneData;
                        if (nodeJ.Deleted)
                        {
                            continue;
                        }

                        vector.Recalculate(nodeI, nodeJ);

                        // Utilities code had:
                        //
                        // See also https://github.com/jimmejardine/qiqqa-open-source/issues/26
#if UNUSED_CODE
                        double strength = SPEED * Math.Min(2, (vector.minimum_extent / (vector.box_distance + 1)));
                        DoPushPull(nodeI, nodeJ, vector, strength);
#else
                        // Qiqqa code chunk alt:
                        double strength = vector.maximum_extent * SPEED * (1 / (vector.box_distance + 1));
                        strength = Math.Min(strength, 5);
                        if (strength > 10)
                        {
                        }
                        // end of Qiqqa alt chunk; looks to me like someone has been fiddling around here...
                        // (including the logline below, which was also not in Utilities codebase...
                        DoPushPull(nodeI, nodeJ, vector, strength);
                        //Logging.Info("REPULSE STRENGTH={0}, box.distance={1}", strength, vector.box_distance);
#endif
                    }
                }
            }

            // Perform the attraction
            if (true)
            {
                int MAX_CONNECTORS = cache_connector_controls.Count;
                for (int i = 0; i < MAX_CONNECTORS; ++i)
                {
                    ConnectorControl connector = cache_connector_controls[i];
                    if (connector.Deleted)
                    {
                        continue;
                    }

                    NodeControlSceneData nodeI = connector.NodeFrom.NodeControlSceneData;
                    NodeControlSceneData nodeJ = connector.NodeTo.NodeControlSceneData;

                    vector.Recalculate(nodeI, nodeJ);

#if UNUSED_CODE
					// Utilities codebase was:
					// See also https://github.com/jimmejardine/qiqqa-open-source/issues/26
	                double strength = -1 * SPEED * (vector.distance / vector.minimum_extent);
	                DoPushPull(nodeI, nodeJ, vector, strength);
#else
                    double strength = -1 * SPEED * (vector.box_distance / 50);
                    DoPushPull(nodeI, nodeJ, vector, strength);
                    //Logging.Info("ATTRACT STRENGTH={0}", strength);
#endif
                }
            }

            NotifySceneRenderingControl();
        }

        private void DoPushPull(NodeControlSceneData nodeI, NodeControlSceneData nodeJ, NodesVector vector, double strength)
        {
            nodeI.SetDeltaCentreX(+strength * vector.unit_x);
            nodeJ.SetDeltaCentreX(-strength * vector.unit_x);

            nodeI.SetDeltaCentreY(+strength * vector.unit_y);
            nodeJ.SetDeltaCentreY(-strength * vector.unit_y);
        }

        private void NotifySceneRenderingControl()
        {
            scene_rendering_control.Dispatcher.Invoke(new Action(() => scene_rendering_control.RecalculateAllNodeControlDimensions()), DispatcherPriority.Background);
        }

        private class NodesVector
        {
            private Rect box_i;
            private Rect box_j;

            public double box_distance;

            public double delta_x;
            public double delta_y;
            public double distance;
            public double angle;
            public double unit_x;
            public double unit_y;

#if UNUSED_CODE
			// Utilities codebase had MIN instead of max:
			// (https://github.com/jimmejardine/qiqqa-open-source/issues/26)
			//
            public double minimum_extent;
#else
            public double maximum_extent;
#endif

            public NodesVector()
            {
            }

            public void Recalculate(NodeControlSceneData nodeI, NodeControlSceneData nodeJ)
            {
                box_i.X = nodeI.Left;
                box_i.Y = nodeI.Top;
                box_i.Width = nodeI.Width;
                box_i.Height = nodeI.Height;
                box_j.X = nodeJ.Left;
                box_j.Y = nodeJ.Top;
                box_j.Width = nodeJ.Width;
                box_j.Height = nodeJ.Height;

                box_distance = BoxDistance.CalculateDistanceBetweenTwoBoxes(box_i, box_j);

#if UNUSED_CODE
				// Utilities codebase had:
				// (https://github.com/jimmejardine/qiqqa-open-source/issues/26)
	            minimum_extent = Math.Min(Math.Min(nodeI.Width, nodeI.Height), Math.Min(nodeJ.Width, nodeJ.Height));
#else
                // Qiqqa codebase had:
                //
                //Logging.Debug(" DIST {0}", box_distance);
                maximum_extent = Math.Max(Math.Max(nodeI.Width, nodeI.Height), Math.Max(nodeJ.Width, nodeJ.Height));
                //minimum_extent = Math.Min(Math.Min(nodeI.Width, nodeI.Height), Math.Min(nodeJ.Width, nodeJ.Height));
#endif

                delta_x = nodeI.CentreX - nodeJ.CentreX;
                delta_y = nodeI.CentreY - nodeJ.CentreY;

                distance = Math.Sqrt(delta_x * delta_x + delta_y * delta_y);
                if (1 > distance) distance = 1;

                angle = Math.Atan2(delta_y, delta_x);

                unit_x = Math.Cos(angle);
                unit_y = Math.Sin(angle);
            }
        }
    }
}
