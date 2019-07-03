using System;
using System.Collections.Generic;
using System.Windows;
using Utilities.GUI.Brainstorm.Nodes.SimpleNodes;

namespace Utilities.GUI.Brainstorm.Nodes
{
    public class NodeContentControlRegistry
    {
        public static NodeContentControlRegistry Instance = new NodeContentControlRegistry();

        Dictionary<Type, Type> registry_controls = new Dictionary<Type, Type>();
        Dictionary<Type, Type> registry_editors = new Dictionary<Type, Type>();
        
        NodeContentControlRegistry()
        {
            RegisterNodeContentControls();
            RegisterNodeEditorControls();      
        }

        public void RegisterNodeContentControl(Type node_content_type, Type node_content_control_type)
        {
            Logging.Info("Registering NodeContentControl {0} for NodeContent {1}", node_content_control_type, node_content_type);
            registry_controls[node_content_type] = node_content_control_type;
        }

        public void RegisterNodeContentEditor(Type node_content_type, Type node_content_editor_type)
        {
            Logging.Info("Registering NodeContentEditor {0} for NodeContent {1}", node_content_editor_type, node_content_type);
            registry_editors[node_content_type] = node_content_editor_type;
        }

        public FrameworkElement GetContentControl(NodeControl nc, object node_content)
        {
            try
            {
                Type node_content_xxx_type = registry_controls[node_content.GetType()];
                return (FrameworkElement)Activator.CreateInstance(node_content_xxx_type, nc, node_content);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem getting a ContentControl for {0}", node_content);
                throw;
            }
        }

        public FrameworkElement GetContentEditor(NodeControl nc, object node_content)
        {
            try
            {
                Type node_content_xxx_type;
                if (registry_editors.TryGetValue(node_content.GetType(), out node_content_xxx_type))
                {
                    return (FrameworkElement)Activator.CreateInstance(node_content_xxx_type, nc, node_content);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem getting a ContentEditor for {0}", node_content);
                return null;
            }
        }

        private void RegisterNodeContentControls()
        {
            // Later these will be moved to an attribute discovery pattern.  At the moment, hardcode them here.
            RegisterNodeContentControl(typeof(StringNodeContent), typeof(StringNodeContentControl));
            RegisterNodeContentControl(typeof(ImageNodeContent), typeof(ImageNodeContentControl));
            RegisterNodeContentControl(typeof(LinkedImageNodeContent), typeof(LinkedImageNodeContentControl));
            RegisterNodeContentControl(typeof(IconNodeContent), typeof(IconNodeContentControl));
            RegisterNodeContentControl(typeof(WebsiteNodeContent), typeof(WebsiteNodeContentControl));
            RegisterNodeContentControl(typeof(FileSystemNodeContent), typeof(FileSystemNodeContentControl));
            RegisterNodeContentControl(typeof(EllipseNodeContent), typeof(EllipseNodeContentControl));
        }

        private void RegisterNodeEditorControls()
        {
            RegisterNodeContentEditor(typeof(StringNodeContent), typeof(StringNodeContentEditor));
        }
    }
}
