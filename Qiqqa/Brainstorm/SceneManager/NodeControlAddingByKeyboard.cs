using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Qiqqa.Brainstorm.Connectors;
using Qiqqa.Brainstorm.Nodes;

namespace Qiqqa.Brainstorm.SceneManager
{
    internal class NodeControlAddingByKeyboard
    {
        private static void GetAdjoiningConnectors(SceneRenderingControl scene_rendering_control, NodeControl node_control_parent, out List<ConnectorControl> connectors_both, out List<ConnectorControl> connectors_to, out List<ConnectorControl> connectors_from)
        {
            connectors_both = new List<ConnectorControl>();
            connectors_to = new List<ConnectorControl>();
            connectors_from = new List<ConnectorControl>();

            // Get the inbound and outbound edges
            foreach (ConnectorControl connector in scene_rendering_control.ObjNodesLayer.Children.OfType<ConnectorControl>())
            {
                if (connector.node_from == node_control_parent)
                {
                    connectors_both.Add(connector);
                    connectors_from.Add(connector);
                }
                if (connector.node_to == node_control_parent)
                {
                    connectors_both.Add(connector);
                    connectors_to.Add(connector);
                }
            }
        }

        public static void AddChildToNodeControl(NodeControl node_control_parent, object content)
        {
            AddChildToNodeControl(node_control_parent, content, true);
        }

        public static void AddChildToNodeControl(NodeControl node_control_parent, object content, bool select_node)
        {
            List<ConnectorControl> connectors_both;
            List<ConnectorControl> connectors_to;
            List<ConnectorControl> connectors_from;
            GetAdjoiningConnectors(node_control_parent.scene_rendering_control, node_control_parent, out connectors_both, out connectors_to, out connectors_from);

            // Get the average connectors direction
            Point direction_inbound = new Point(0, 0);
            int denominator = 0;
            foreach (ConnectorControl connector in connectors_to)
            {
                if (connector.Deleted)
                {
                    continue;
                }

                direction_inbound.X += connector.node_to.NodeControlSceneData.CentreX - connector.node_from.NodeControlSceneData.CentreX;
                direction_inbound.Y += connector.node_to.NodeControlSceneData.CentreY - connector.node_from.NodeControlSceneData.CentreY;
                denominator += 1;
            }

            if (0 < denominator)
            {
                direction_inbound.X /= denominator;
                direction_inbound.Y /= denominator;
            }
            else
            {
                double angle = Math.PI * 140.0 / 180.0 * (1 + connectors_from.Count);
                double max_dimension = Math.Max(node_control_parent.scene_data.Width, node_control_parent.scene_data.Height);
                direction_inbound.X = 1.5 * max_dimension * Math.Cos(angle);
                direction_inbound.Y = 1.5 * max_dimension * Math.Sin(angle);
            }

            // Pick an outward direction
            Point direction_outbound = new Point(direction_inbound.X, direction_inbound.Y);
            double total_divergence = 0.0;
            if (connectors_to.Count > 0)
            {
                double current_divergence = Math.PI / 4;
                int i = connectors_from.Count;
                while (i > 0)
                {
                    int remainder = i % 2;
                    if (remainder > 0)
                    {
                        total_divergence += current_divergence;
                    }
                    else
                    {
                        total_divergence -= current_divergence;
                    }

                    current_divergence = current_divergence / 2;
                    i = i / 2;
                }
            }

            // Rotate the direction
            Point direction_outbound_rotated = new Point();
            direction_outbound_rotated.X = direction_outbound.X * Math.Cos(total_divergence) + direction_outbound.Y * Math.Sin(total_divergence);
            direction_outbound_rotated.Y = -direction_outbound.X * Math.Sin(total_divergence) + direction_outbound.Y * Math.Cos(total_divergence);

            double CHILD_SHRINKAGE_FACTOR = 0.8;

            double left = node_control_parent.scene_data.CentreX + direction_outbound_rotated.X * CHILD_SHRINKAGE_FACTOR;
            double top = node_control_parent.scene_data.CentreY + direction_outbound_rotated.Y * CHILD_SHRINKAGE_FACTOR;

            //xxxxxxxxxxxxxxxxxxxxxxxxx
            double width = node_control_parent.scene_data.Width * CHILD_SHRINKAGE_FACTOR;
            double height = node_control_parent.scene_data.Height * CHILD_SHRINKAGE_FACTOR;

            // Create a node at the outbound direction            
            NodeControl node_new = node_control_parent.scene_rendering_control.AddNewNodeControl(content, left, top, width, height);

            if (node_new != node_control_parent)
            {
                // Check that we are not connected to the node control if it is being reused
                bool already_connected = false;
                if (!already_connected)
                {
                    foreach (ConnectorControl cc in connectors_to)
                    {
                        if (cc.node_from == node_new)
                        {
                            already_connected = true;
                            break;
                        }
                    }
                }
                if (!already_connected)
                {
                    foreach (ConnectorControl cc in connectors_from)
                    {
                        if (cc.node_to == node_new)
                        {
                            already_connected = true;
                            break;
                        }
                    }
                }

                // Create a link to the new child
                if (!already_connected)
                {
                    ConnectorControl connector_new = new ConnectorControl(node_control_parent.scene_rendering_control);
                    connector_new.SetNodes(node_control_parent, node_new);
                    node_control_parent.scene_rendering_control.AddNewConnectorControl(connector_new);
                }

                // Choose this new node            
                if (select_node)
                {
                    node_control_parent.scene_rendering_control.SetSelectedNodeControl(node_new, false);
                }
            }
        }

        internal static void AddSiblingToNodeControl(NodeControl node_control)
        {
            List<ConnectorControl> connectors_both;
            List<ConnectorControl> connectors_to;
            List<ConnectorControl> connectors_from;
            GetAdjoiningConnectors(node_control.scene_rendering_control, node_control, out connectors_both, out connectors_to, out connectors_from);

            double left = node_control.scene_data.CentreX;
            double top = node_control.scene_data.CentreY;
            double width = node_control.scene_data.Width;
            double height = node_control.scene_data.Height;

            top += 1.5 * height;

            NodeControlSceneData scene_data = new NodeControlSceneData();
            object content = new StringNodeContent("Sibling node");
            NodeControl node_new = node_control.scene_rendering_control.AddNewNodeControl(content, left, top, width, height);

            if (connectors_to.Count > 0)
            {
                NodeControl node_control_parent = connectors_to[connectors_to.Count - 1].node_from;

                ConnectorControl connector_new = new ConnectorControl(node_control.scene_rendering_control);
                connector_new.SetNodes(node_control_parent, node_new);
                node_control.scene_rendering_control.AddNewConnectorControl(connector_new);
            }

            node_control.scene_rendering_control.SetSelectedNodeControl(node_new, false);
        }
    }
}
