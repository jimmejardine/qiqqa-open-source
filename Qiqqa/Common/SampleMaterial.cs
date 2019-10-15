using System.Windows.Forms;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Qiqqa.Common
{
    public class SampleMaterial
    {
        public static readonly string Brainstorm = Path.GetFullPath(Path.Combine(Configuration.ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"SampleMaterial/FinanceSample.brain"));
    }
}
