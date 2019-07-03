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
        object thread_lock = new object();
        Thread active_thread;

        SceneRenderingControl scene_rendering_control;
        
        public AutoArranger(SceneRenderingControl scene_rendering_control)        
        {
            this.scene_rendering_control = scene_rendering_control;
        }

        public void Enabled(bool enabled)
        {
            lock (thread_lock)
            {
                if (enabled)
                {
                    // USER WANTS TO ENABLE
                    if (null == active_thread)
                    {
                        active_thread = new Thread(BackgroundThread);
                        active_thread.IsBackground = true;
                        active_thread.Priority = ThreadPriority.Lowest;
                        active_thread.Start(active_thread);
                    }
                }

                else
                {
                    // USER WANTS TO DISABLE
                    active_thread = null;
                }
            }
        }

        private void BackgroundThread(object thread_object)
        {
            Thread thread = (Thread)thread_object;            
            Logging.Info("Thread {0} has started", thread.ManagedThreadId);

            while (true)
            {
                lock (thread_lock)
                {
                    if (thread != active_thread)
                    {
                        break;
                    }
                }

                DoLayout();

                Thread.Sleep(30);
            }

            Logging.Info("Thread {0} has exited", thread.ManagedThreadId);
        }

        DateTime cache_scene_changed_timestamp = DateTime.MinValue;
        List<NodeControl> cache_node_controls = new List<NodeControl>();
        List<ConnectorControl> cache_connector_controls = new List<ConnectorControl>();
        
        private void DoLayout()
        {
            int SPEED = 1;

            // If the nodes and connectors have changed, recache them!
            if (cache_scene_changed_timestamp != this.scene_rendering_control.SceneChangedTimestamp)
            {
                Logging.Info("Scene has changed, so autolayout is recaching.");
                cache_scene_changed_timestamp = this.scene_rendering_control.SceneChangedTimestamp;
                cache_node_controls = new List<NodeControl>(this.scene_rendering_control.NodeControls);
                cache_connector_controls = new List<ConnectorControl>(this.scene_rendering_control.ConnectorControlManager.ConnectorControls);
            }

            // We reuse this so that it is memory allocation time efficient
            NodesVector vector = new NodesVector();            

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

                        double strength = vector.maximum_extent * SPEED * (1 / (vector.box_distance + 1));
                        strength = Math.Min(strength, 5);
                        if (strength > 10)
                        {
                        }
                        DoPushPull(nodeI, nodeJ, vector, strength);
                        //Logging.Info("REPULSE STRENGTH={0}, box.distance={1}", strength, vector.box_distance);
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

                    double strength = -1 * SPEED * (vector.box_distance / 50);                    
                    DoPushPull(nodeI, nodeJ, vector, strength);
                    //Logging.Info("ATTRACT STRENGTH={0}", strength);
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

        class NodesVector
        {
            Rect box_i;
            Rect box_j;

            public double box_distance;

            public double delta_x;
            public double delta_y;
            public double distance;
            public double angle;
            public double unit_x;
            public double unit_y;

            public double maximum_extent;

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
                //Logging.Debug(" DIST {0}", box_distance);
                maximum_extent = Math.Max(Math.Max(nodeI.Width, nodeI.Height), Math.Max(nodeJ.Width, nodeJ.Height));
                //minimum_extent = Math.Min(Math.Min(nodeI.Width, nodeI.Height), Math.Min(nodeJ.Width, nodeJ.Height));

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
