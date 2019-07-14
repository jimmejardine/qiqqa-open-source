Contains the Inno Setup files for generating the installer exe.  Includes several scripts for checking dependencies, needed for the .NET 4.0 dependency.

Also contains the `signtool.exe` from the Windows (7.1) SDK, for signing our package, so we don't get a unknown publisher warning on kicking off the installer.

The Content folder contains the license file used in the installer.

`ISCC` is the **Inno Setup compiler** which is kicked off by `nant`, this is instead of the Inno Setup IDE, which can also be used.

`setup.iss` is our Inno Setup file, which pulls in the .NET 4.0 dependency scripts.  It requires several settings in the command line (see the top of the file).

## .NET 4.0 Dependency requirements

Minimum Windows Versions - currently assumes that Windows XP SP3 is the minimum ito:

```
Major Version=5
Minor Version=1
SP=3
```

Had a look at Windows version numbers, and tried to match them up the the .NET dependencies.  Not too simple, so went with the assumption above.

.NET 4.0 supported by (from .NET 4.0 readme):
- Windows XP SP3 (x86=v5.1SP3) [no mention of the 64 bit version]
- Windows Server 2003 SP2 (v5.2 SP2)
- Windows Vista SP1 (v6.0 SP1)
- Windows 7 (v6.1)
- Windows Server 2008 (not supported on Server Core Role) (v6.0)
- Windows Server 2008 R2 (not supported on Server Core Role) (v6.1)

### Weirdness:

- The .NET Framework 4 Client Profile is not supported on ia64 - .NET 4.0 readme

- Windows XP SP3 required, no mention of Windows XP x64 SP version which currently only has SP2.  And the x64 RTM is based on Windows Server 2003 SP1, ...

- Might be a problem that Vista requires SP1 (v6.1SP1), but that Windows Server 2008 is supported with no SP (v6.0) only???


---

.NET 4.0 has dependency on Windows Installer 3.1.  Windows installer versions bundled with Windows:

+-----+-----------------------------------------------------------+
| 3.1 | - Windows XP SP3                                          |
|     | - Windows Server 2003 SP1, SP2                            |
|     | - Windows XP Professional x64 Edition RTM, SP2            |
+-----+-----------------------------------------------------------+
|  4  | - Windows Vista RTM, SP1                                  |
|     | - Windows Server 2008 RTM                                 |
+-----+-----------------------------------------------------------+
| 4.5 | - Windows Vista SP2                                       |
|     | - Windows Server 2008 SP2                                 |
+-----+-----------------------------------------------------------+
|  5  | - Windows 7 RTM                                           |
|     | - Windows Server 2008 R2 RTM                              |
+-----+-----------------------------------------------------------+

(so requirements for .NET 4.0 are stricter than for Windows Installer 3.1 - don't bother checking for Windows Installer version)

Windows Versions (http://msdn.microsoft.com/en-us/library/ms724834(VS.85).aspx):
+----------------------------+------------+
| Windows 7   | 6.1 |
+----------------------------+------------+
| Windows Server 2008 R2  | 6.1 |
+----------------------------+------------+
| Windows Server 2008 | 6.0 |
+----------------------------+------------+
| Windows Vista   | 6.0 |
+----------------------------+------------+
| Windows Server 2003 R2  | 5.2 |
+----------------------------+------------+
| Windows Server 2003 | 5.2 |
+----------------------------+------------+
| Windows XP 64-Bit Edition   | 5.2 |
+----------------------------+------------+
| Windows XP  | 5.1 |
+----------------------------+------------+
| Windows 2000  |   5 |
+----------------------------+------------+
