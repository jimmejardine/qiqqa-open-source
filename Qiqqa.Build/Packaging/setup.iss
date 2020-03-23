; Constants
#define AppName 'Qiqqa'
#define ExeName 'Qiqqa.exe'
#define AppId '99AF0582-482B-4E5E-BB11-675354BF5E77'
#define AppIcon '.\Icons\Qiqqa.ico'
#define AppSupportURL 'http://www.qiqqa.com/Feedback'
#define AppHelpURL 'http://www.qiqqa.com/Help'
; -do a silent .NET 4.0 install (Sets passive mode; displays the progress bar to indicate that installation is in progress, but does not display any prompts or error messages to the user.)
#define dotnet_Passive

; Constants passed in command line
; #define AppVersion '1.0'
; #define AppFullVersion '1.0.3156.4513'
; #define AppSource 'D:\qiqqa\src\client\Qiqqa\bin\Release\*'
; #define OutputDir 'D:\qiqqa\src\client\Qiqqa.Build\Packages\Latest'
; #define OutputBaseFilename 'setup'

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppID={{{#AppId}}
AppName={#AppName}
AppVersion={#AppVersion}
;AppVerName=Qiqqa 1
AppPublisher=Quantisle Ltd.
AppPublisherURL=http://www.qiqqa.com/
AppSupportURL={#AppSupportURL}
AppUpdatesURL=http://www.qiqqa.com/
AppCopyright=Copyright © Quantisle Ltd 2010-2019
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
LicenseFile=.\Content\license.txt
OutputDir={#OutputDir}
OutputBaseFilename={#OutputBaseFilename}
SetupIconFile={#AppIcon}
Compression=lzma/Max
SolidCompression=yes
SetupLogging=yes
PrivilegesRequired=admin
; the Windows file version as reported by the Properties of the generated setup.exe:
VersionInfoVersion={#AppFullVersion}

; from: http://www.kinook.com/blog/?p=53
DefaultDirName={code:DefDirRoot}\{#AppName}
UsePreviousAppDir=no
DisableDirPage=no

Uninstallable=yes
UninstallDisplayName={#AppName}
UninstallLogMode=append
UninstallDisplayIcon={app}\{#ExeName}

WizardImageBackColor=clBlack
WizardImageFile=WizardLargeImage.bmp
WizardSmallImageFile=WizardSmallImage.bmp

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#AppSource}"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "Include\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "Icons\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\Qiqqa "; Filename: "{app}\{#ExeName}"
Name: "{group}\Qiqqa online help"; Filename: "{#AppHelpURL}"; IconFilename: "{app}\startmenu_help.ico"
Name: "{group}\Qiqqa online support"; Filename: "{#AppSupportURL}"; IconFilename: "{app}\startmenu_support.ico"
Name: "{commondesktop}\Qiqqa"; Filename: "{app}\{#ExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Qiqqa"; Filename: "{app}\{#ExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\couninst.exe"
Filename: "{app}\{#ExeName}"; Description: "{cm:LaunchProgram,Qiqqa}"; Flags: nowait postinstall skipifsilent

#include "scripts\startup.iss"
#include "scripts\products.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"
#include "scripts\products\dotnetfx40client.iss"
//#include "scripts\products\msi31.iss"
//#include "scripts\products\msi45.iss"
//#include "scripts\products\dotnetfx40full.iss"
//#include "scripts\products\dotnetfx35sp1.iss"


[CustomMessages]
winxpsp2_title=Windows XP Service Pack 2

[Code]
function IsRegularUser(): Boolean;
begin
    Result := not (IsAdminLoggedOn or IsPowerUserLoggedOn);
end;

function DefDirRoot(Param: String): String;
begin
    if IsRegularUser then
        Result := ExpandConstant('{localappdata}\Quantisle\')
    else
        Result := ExpandConstant('{pf}')
end;

function UninstallPreviousVersion(UninstallRegistryBaseKey: Integer; UninstallRegistryLocation: String; UninstallWorkingDir: String): Boolean;
var
    oldVersion: String;
    uninstaller: String;
    ErrorCode: Integer;
    shouldUninstallPrevious: Boolean;
begin
    //  check if already running (check all possible running exe's)
    while (IsAppRunning('{#ExeName}') or IsAppRunning('pdfdraw.exe') or IsAppRunning('QiqqaOCR.exe')) do
    begin
        if (MsgBox( '{#AppName} is currently running, please close it and retry.  This can also happen if you have renamed the Qiqqa setup.exe program to "Qiqqa.exe".  Alternatively you can terminate any running processes called "Qiqqa.exe", "QiqqaOCR.exe" or "pdfdraw.exe".', mbError, MB_RETRYCANCEL ) = IDCANCEL) then
        begin
          Result := False;
          Exit;
        end;
    end;

    //  if the app isn't installed (any version) then all is well
    if not RegKeyExists(UninstallRegistryBaseKey, UninstallRegistryLocation) then
    begin
        Result := True;
        Exit;
    end;

    //  if we get here, the there is an old version lying around, confirm that the user wants to uninstall, or close the program by returning false
    //  compare versions - are they currently an older / newer / current version - let them know, but allow an uninstall
    shouldUninstallPrevious := false;
    //  check version of already installed
    RegQueryStringValue(UninstallRegistryBaseKey, UninstallRegistryLocation, 'DisplayVersion', oldVersion);
    //  trying to install an old version
    if (CompareVersion(oldVersion, '{#AppVersion}') > 0) then
    begin
        if ((MsgBox('A newer version of {#AppName} is already installed ({#AppName} version ' + oldVersion + ').'+#13+#13+'Are you sure you would like to remove the newer version of Qiqqa to install {#AppName} version {#AppVersion}?'+#13+#13+'(Note: your existing data will NOT be affected by the uninstall, and will be waiting for you after the new installation is complete.)', mbConfirmation, MB_YESNO) = IDYES)) then
        begin
            shouldUninstallPrevious := true;
        end;
    end
    //  trying to install the same version
    else if ((CompareVersion(oldVersion, '{#AppVersion}') = 0)) then
    begin
        if ((MsgBox('This version of {#AppName} is already installed ({#AppName} version ' + oldVersion + ').  The current installation must be removed to be able to reinstall.  This will not erase your Qiqqa database.'+#13+#13+'Proceed with the uninstall now?'+#13+#13+'(Note: your existing data will NOT be affected by the uninstall, and will be waiting for you after the new installation is complete.)', mbConfirmation, MB_YESNO) = IDYES)) then
        begin
            shouldUninstallPrevious := true;
        end;
    end
    else
    //  trying to install a new version
    begin
        if ((MsgBox('An older version of {#AppName} is installed ({#AppName} version ' + oldVersion + '), and must be removed before the new version can be installed.  This will not erase your Qiqqa database.'+#13+#13+'Proceed with the uninstall now?'+#13+#13+'(Note: your existing data will NOT be affected by the uninstall, and will be waiting for you after the new installation is complete.)', mbConfirmation, MB_YESNO) = IDYES)) then
        begin
            shouldUninstallPrevious := true;
        end;
    end;

    //  ok, should we do the uninstall?
    if (shouldUninstallPrevious) then
    begin
      RegQueryStringValue(UninstallRegistryBaseKey, UninstallRegistryLocation, 'UninstallString', uninstaller);
      uninstaller := RemoveQuotes(uninstaller);
      Exec(uninstaller, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, ErrorCode);
      if ((ErrorCode <> 0)) then
      begin
        MsgBox('Failed to uninstall {#AppName} version ' + oldVersion + '. Please restart Windows and run setup again.', mbError, MB_OK);
        Result := False;
        Exit;
      end;
    end
    else
    begin
        // if they don't want to uninstall then return false, the program should end
        Result := False;
        Exit;
    end;

    //  all good to go
    Result := True;
end;

// =======================================
// Does all the startup checks and complains if there is a problem (also returns false then).
// =======================================
function CanStartup(): Boolean;
begin
    // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    //  Simple startup checking
    //  (http://www.lextm.com/2007/08/inno-setup-script-sample-for-version.html)
    // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    //  in safe mode?
    if IsSafeModeBoot then
        begin
        MsgBox( 'Cannot install in Windows Safe Mode.  The installer will now exit.', mbError, MB_OK);
        Result := False;
        Exit;
    end;

    //  remove the previous installer version (if installed) - check both the CURRENT_USER & LOCAL_MACHINE since depends on admin rights
    if (not UninstallPreviousVersion(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{{#AppId}}_is1', '') or
        not UninstallPreviousVersion(HKEY_CURRENT_USER, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{{#AppId}}_is1', '')) then
    begin
        Result := False;
        Exit;
    end;

    Result := True;
end;

function InitializeSetup(): Boolean;
begin
  //  STARTUP CHECKS
  if not CanStartup() then begin
    exit;
  end;

  //  .NET 3.5.1 DEPENDENCY CHECKS
	//init windows version
	initwinversion();
	//check if dotnetfx40client can be installed on this OS
	if not minwinversion(5, 1) then begin
		MsgBox(FmtMessage(CustomMessage('depinstall_missing'), [CustomMessage('winxpsp2_title')]), mbError, MB_OK);
		exit;
	end;
	// the following doesn't fail if major!=5 and minor!=1
	// we require at least sp2 (this is a .net 3.5.1 requirement)
	if not minwinspversion(5, 1, 2) then begin
		MsgBox(FmtMessage(CustomMessage('depinstall_missing'), [CustomMessage('winxpsp2_title')]), mbError, MB_OK);
		exit;
	end;

	// Windows Installers
	//msi31('3.1');
	//msi45('4.5');

	// .NET 3.5.1
	//dotnetfx35sp1();

	// Check that .NET4 client profile is here...
	dotnetfx40client(false);

// // If no .NET framework found, install the smallest
//	if not dotnetfx40client(true) then
//	    if not dotnetfx40full(true) then begin
//	        dotnetfx40client(false);
//      end
//	// Alternatively:
//	dotnetfx40full();

	Result := true;
end;

function InitializeUninstall(): Boolean;
begin
  if IsSafeModeBoot then
  begin
    MsgBox( 'Cannot uninstall in Windows Safe Mode.', mbError, MB_OK);
    Result := False;
    Exit;
  end;

  // check if app is running
  if (IsAppRunningU('{#ExeName}') or IsAppRunningU('pdfdraw.exe') or IsAppRunningU('QiqqaOCR.exe')) then
  begin
    MsgBox( '{#AppName} is running, please close it and run uninstall again.', mbError, MB_OK );
    Result := false;
    Exit;
  end
  else Result := true;

  // Unload the DLL, otherwise the dll psvince is not deleted
  UnloadDLL(ExpandConstant('{app}\psvince.dll'));
end;

procedure DeinitializeUninstall();
var
	ErrorCode: Integer;
begin
	if UninstallSilent then
	begin
	end
	else
	begin
		if MsgBox('We are sorry to see you go!  Would you mind letting us know why you are uninstalling {#AppName}: it will really help us improve?  If Qiqqa just does not start, you probably need to update or repair your Microsoft .NET 4 Client Profile.', mbConfirmation, MB_YESNO) = idYes then
		begin
      ShellExec('open', 'http://www.qiqqa.com/UninstallFeedback', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
		end
		else
		begin
      ShellExec('open', 'http://www.pdfhighlights.com/?utm_source=qiqqa&utm_medium=uninstall&utm_campaign=uninstall', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
    end
	end
end;

// Uncomment the line below to see the preprocessed code
// #expr SaveToFile(AddBackslash(SourcePath) + "Preprocessed.iss")
