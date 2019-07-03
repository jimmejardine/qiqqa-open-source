using System.Reflection;
using System.Windows.Controls;

namespace Qiqqa.StartPage.Formatting
{
    // I wish I knew how to disable syncfusion styles without having to create these "phantom subclasses" so the template does not match!
    [Obfuscation(Feature = "renaming", ApplyToMembers = false)]
    class LocalButton : Button
    {
    }
}
