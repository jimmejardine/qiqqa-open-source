using System;
using System.Collections.Generic;
using System.Windows;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.SceneManager
{
    [Serializable]
    public class BrainstormFileFormat : DictionaryBasedObject
    {
        public BrainstormMetadata BrainstormMetadata
        {
            get => this["BrainstormMetadata"] as BrainstormMetadata;
            set => this["BrainstormMetadata"] = value;
        }

        public List<NodeControlSceneData> NodesV1
        {
            get => this["NodesV1"] as List<NodeControlSceneData>;
            set => this["NodesV1"] = value;
        }

        public List<ConnectorV1> ConnectorsV1
        {
            get => this["ConnectorsV1"] as List<ConnectorV1>;
            set => this["ConnectorsV1"] = value;
        }

        public Point? CurrentViewport
        {
            get => this["CurrentViewport"] as Point?;
            set => this["CurrentViewport"] = value;
        }

        public double? CurrentScale
        {
            get => this["CurrentScale"] as double?;
            set => this["CurrentScale"] = value;
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
