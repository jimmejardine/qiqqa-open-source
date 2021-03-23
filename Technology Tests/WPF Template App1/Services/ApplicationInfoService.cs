using System;
using System.Diagnostics;
using System.Reflection;

using OSVersionHelper;

using Windows.ApplicationModel;

using WPF_Template_App1.Contracts.Services;

namespace WPF_Template_App1.Services
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        public ApplicationInfoService()
        {
        }

        public Version GetVersion()
        {
            if (WindowsVersionHelper.HasPackageIdentity)
            {
                // Packaged application
                // Set the app version in WPF Template App1.Packaging > Package.appxmanifest > Packaging > PackageVersion
                var packageVersion = Package.Current.Id.Version;
                return new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
            }

            // Set the app version in WPF Template App1 > Properties > Package > PackageVersion
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var version = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
            return new Version(version);
        }
    }
}
