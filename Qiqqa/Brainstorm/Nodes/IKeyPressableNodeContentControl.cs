using System.Windows.Input;

namespace Qiqqa.Brainstorm.Nodes
{
    interface IKeyPressableNodeContentControl
    {
        void ProcessKeyPress(KeyEventArgs e);
    }
}
