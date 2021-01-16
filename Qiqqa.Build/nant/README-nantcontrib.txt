NAntContrib

What is it?
-----------
NAntContrib is the project for tasks and tools that haven't made it into the main NAnt distribution 
yet or for whatever reason don't belong there.


How to use NAntContrib tasks in NAnt?
-------------------------------------
In order to use NAntContrib tasks in NAnt, you can use one of the following procedures:

1. Use the <loadtasks> task in your build file(s) to tell NAnt which assembly to scan for tasks

For example:

<project name="NAntContrib" default"test">
    <target name="test">
        <loadtasks assembly="c:/nantcontrib-0.86/bin/NAnt.Contrib.Tasks.dll" />
        ...
    </target>
</project>

In order to avoid updating all build files when you move the NAnt.Contrib.Tasks assembly to another
directory, you could register the directory containing the NAnt.Contrib.Tasks assembly in an 
environment variable and then use the value of that environment variable in your build files.

For example:

<project name="NAntContrib" default"test">
    <target name="test">
        <loadtasks assembly="${path::combine(environment::get-variable('NANTCONTRIB_DIR'), 'bin/NAnt.Contrib.Tasks.dll')}" />
        ...
    </target>
</project>

This example assumes you've registered an environment variable named "NANTCONTRIB_DIR", holding the 
path to NAntContrib (eg. c:\nantcontrib-0.86).

2. Copy the content of the NAntContrib bin directory to the NAnt directory

In order to make certain tasks available to every build file, you can copy the
extensions assembly to the following directory:

    <nant>\bin\extensions\common\neutral\NAntContrib

and copy the third-party assemblies to directory:

    <nant>\bin\lib\common\neutral\NAntContrib

You should eventually end up with the following directory structure:

<nant>\
    bin\
        extensions\
            common\
                neutral\
                    NAntContrib\
                        NAnt.Contrib.Tasks.dll
                        NAnt.Contrib.Tasks.pdb (not available for releases)
                        NAnt.Contrib.Tasks.xml
        lib\
            common\
                neutral\
                    NAntContrib\
                        CollectionGen.dll
                        Interop.MsmMergeTypeLib.dll
                        Interop.StarTeam.dll
                        Interop.WindowsInstaller.dll
                        SLiNgshoT.Core.dll
                        SourceSafe.Interop.dll

Note: you might have to manually create the directories.

3. Modify NAnt configuration file (not recommended)

An <include> element can be added to the <task-assemblies> node in the <framework> node for the
.NET Framework version that you intend to use. The "name" attribute of the <include> element 
should hold the absolute path to NAnt.Contrib.Tasks.dll.

For example:

    <framework 
            name="net-1.1" 
                family="net" 
                version="1.1"
                description="Microsoft .NET Framework 1.1"
        ....
    >
            <task-assemblies>
            ...
            <include name="c:/nantcontrib-0.86/bin/NAnt.Contrib.Tasks.dll" />
            ...
                </task-assemblies>
        ....
    </framework>

You would need to repeat this for all .NET Framework versions that you intend to use.

Note: The NAnt configuration file (NAnt.exe.config) is considered internal, and might change without
      noticed.


How to build.
-------------
To build NAntContrib, the following procedure should be followed:


1. Download and extract a binary distribution of NAnt from http://nant.sourceforge.net

2. Change to the NAntContrib directory

3. Run NAntContrib.build using the version of NAnt that you downloaded

    eg.  c:\NAnt-0.86\bin\NAnt.exe -D:nant.dir=c:\NAnt -f:NAntContrib.build

Note: 

These instructions only apply to the source distribution of NAntContrib, as the binary distribution 
contains pre-built assemblies.


Documentation
-------------
Documentation is available in HTML format, in the doc/ directory.
