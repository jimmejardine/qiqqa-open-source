using System;
using System.Collections.Generic;
using System.Windows;
using Qiqqa.Brainstorm.Nodes;
using Utilities.Misc;

namespace Qiqqa.Brainstorm.SceneManager
{
    [Serializable]
    public class BrainstormFileFormat : DictionaryBasedObject
    {
        public BrainstormMetadata BrainstormMetadata
        {
            get { return this["BrainstormMetadata"] as BrainstormMetadata; }
            set { this["BrainstormMetadata"] = value; }
        }
        
        public List<NodeControlSceneData> NodesV1
        {
            get { return this["NodesV1"] as List<NodeControlSceneData>; }
            set { this["NodesV1"] = value; }
        }

        public List<ConnectorV1> ConnectorsV1
        {
            get { return this["ConnectorsV1"] as List<ConnectorV1>; }
            set { this["ConnectorsV1"] = value; }
        }

        public Point? CurrentViewport
        {
            get { return this["CurrentViewport"] as Point?; }
            set { this["CurrentViewport"] = value; }
        }

        public double? CurrentScale
        {
            get { return this["CurrentScale"] as double?; }
            set { this["CurrentScale"] = value; }
        }

        [Serializable]
        public class ConnectorV1
        {
            public Guid guid_connector;
            public Guid guid_node_from;
            public Guid guid_node_to;
            public bool deleted;
        }
    }
}
