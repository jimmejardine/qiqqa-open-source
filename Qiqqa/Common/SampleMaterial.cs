using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Common
{
    public static class SampleMaterial
    {
        public static readonly string Brainstorm = Path.GetFullPath(Path.Combine(Configuration.ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"SampleMaterial/FinanceSample.brain"));
    }
}
