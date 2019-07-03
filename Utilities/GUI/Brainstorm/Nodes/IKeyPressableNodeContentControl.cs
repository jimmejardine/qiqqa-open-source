using System.Windows.Input;

namespace Utilities.GUI.Brainstorm.Nodes
{
    interface IKeyPressableNodeContentControl
    {
        void ProcessKeyPress(KeyEventArgs e);
    }
}
