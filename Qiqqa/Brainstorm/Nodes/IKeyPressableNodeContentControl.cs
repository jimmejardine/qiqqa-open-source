using System.Windows.Input;

namespace Qiqqa.Brainstorm.Nodes
{
    internal interface IKeyPressableNodeContentControl
    {
        void ProcessKeyPress(KeyEventArgs e);
    }
}
