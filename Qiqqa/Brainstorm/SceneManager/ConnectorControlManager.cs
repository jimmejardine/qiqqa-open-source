using System.Collections.Generic;
using Qiqqa.Brainstorm.Connectors;
using Qiqqa.Brainstorm.Nodes;
using Utilities.Collections;

namespace Qiqqa.Brainstorm.SceneManager
{
    public class ConnectorControlManager
    {
        List<ConnectorControl> connector_controls = new List<ConnectorControl>();

        // Lookups for directional connections
        MultiMapSet<NodeControl, ConnectorControl> links_from_to = new MultiMapSet<NodeControl, ConnectorControl>();
        MultiMapSet<NodeControl, ConnectorControl> links_to_from = new MultiMapSet<NodeControl, ConnectorControl>();

        public void Clear()
        {
            connector_controls.Clear();

            links_from_to.Clear();
            links_to_from.Clear();
        }
        
        public void Add(ConnectorControl connector_control)
        {
            connector_controls.Add(connector_control);

            links_from_to.Add(connector_control.node_from, connector_control);
            links_from_to.Add(connector_control.node_to, connector_control);
        }

        public IEnumerable<ConnectorControl> ConnectorControls
        {
            get
            {
                return connector_controls;
            }
        }

        public IEnumerable<ConnectorControl> GetNodesFrom(NodeControl node_control)
        {
            return links_from_to.Get(node_control);
        }

        public IEnumerable<ConnectorControl> GetNodesTo(NodeControl node_control)
        {
            return links_to_from.Get(node_control);
        }
    }
}
