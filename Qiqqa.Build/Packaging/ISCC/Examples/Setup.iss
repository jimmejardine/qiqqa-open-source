; Inno Setup
; Copyright (C) 1997-2009 Jordan Russell. All rights reserved.
; Portions by Martijn Laan
; For conditions of distribution and use, see LICENSE.TXT.
;
; Inno Setup QuickStart Pack Setup script by Martijn Laan
;
; $jrsoftware: ispack/setup.iss,v 1.97 2010/10/30 21:03:22 mlaan Exp $

[Setup]
AppName=Inno Setup QuickStart Pack
AppId=Inno Setup 5
AppVersion=5.4.0
AppPublisher=Martijn Laan
AppPublisherURL=http://www.innosetup.com/
AppSupportURL=http://www.innosetup.com/
AppUpdatesURL=http://www.innosetup.com/
AppMutex=InnoSetupCompilerAppMutex,Global\InnoSetupCompilerAppMutex
MinVersion=4.1,
DefaultDirName={pf}\Inno Setup 5
DefaultGroupName=Inno Setup 5
AllowNoIcons=yes
Compression=lzma2/ultra
InternalCompressLevel=ultra
SolidCompression=yes
UninstallDisplayIcon={app}\Compil32.exe
LicenseFile=isfiles\license.txt
AppModifyPath="{app}\Ispack-setup.exe" /modify=1
WizardImageFile=compiler:WizModernImage-IS.bmp
WizardSmallImageFile=compiler:WizModernSmallImage-IS.bmp
SetupIconFile=Setup.ico
SignTool=ispacksigntool
SignedUninstaller=yes

[Messages]
AboutSetupNote=Inno Setup Preprocessor home page:%nhttp://ispp.sourceforge.net/

[Tasks]
Name: desktopicon; Description: "{cm:CreateDesktopIcon}"
;Name: fileassoc; Description: "{cm:AssocFileExtension,Inno Setup,.iss}"

[Files]
;first the files used by [Code] so these can be quickly decompressed despite solid compression
Source: "otherfiles\IDE.ico"; Flags: dontcopy
Source: "otherfiles\ISPP.ico"; Flags: dontcopy
Source: "otherfiles\ISCrypt.ico"; Flags: dontcopy
Source: "isxdlfiles\isxdl.dll"; Flags: dontcopy
Source: "isfiles\WizModernSmallImage-IS.bmp"; Flags: dontcopy
;other files
Source: "isfiles\license.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\ISetup.chm"; DestDir: "{app}"; Flags: ignoreversion
Source: "isppfiles\ISPP.chm"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\Compil32.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\isscint.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\ISCC.exe"; DestDir: "{app}"; Flags: ignoreversion; Check: not ISPPCheck
Source: "isppfiles\ISPPCC.exe"; DestDir: "{app}"; DestName: "ISCC.exe"; Flags: ignoreversion; Check: ISPPCheck
Source: "isfiles\ISCmplr.dll"; DestDir: "{app}"; Flags: ignoreversion; Check: not ISPPCheck
Source: "isppfiles\ISCmplr.dll"; DestDir: "{app}"; Flags: ignoreversion; Check: ISPPCheck
Source: "isfiles\ISCmplr.dll"; DestDir: "{app}"; DestName: "ISCmplr.dls"; Flags: ignoreversion; Check: ISPPCheck
Source: "isppfiles\Builtins.iss"; DestDir: "{app}"; Flags: ignoreversion; Check: ISPPCheck
Source: "isfiles\Setup.e32"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\SetupLdr.e32"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\Default.isl"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\Languages\Basque.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\BrazilianPortuguese.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Catalan.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Czech.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Danish.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Dutch.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Finnish.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\French.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\German.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Hebrew.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Hungarian.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Italian.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Japanese.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Norwegian.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Polish.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Portuguese.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Russian.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Slovak.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Slovenian.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\Languages\Spanish.isl"; DestDir: "{app}\Languages"; Flags: ignoreversion
Source: "isfiles\WizModernImage.bmp"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\WizModernImage-IS.bmp"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\WizModernSmallImage.bmp"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\WizModernSmallImage-IS.bmp"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\iszlib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\isunzlib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\isbzip.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\isbunzip.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\islzma.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\islzma32.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\islzma64.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\whatsnew.htm"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\isfaq.htm"; DestDir: "{app}"; Flags: ignoreversion
Source: "isfiles\Examples\Example1.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\Example2.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\Example3.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\64Bit.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\64BitThreeArch.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\64BitTwoArch.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\Components.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\Languages.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\MyProg.exe"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\MyProg-x64.exe"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\MyProg-IA64.exe"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\MyProg.chm"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\Readme.txt"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\Readme-Dutch.txt"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\Readme-German.txt"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\CodeExample1.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\CodeDlg.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\CodeClasses.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\CodeDll.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\CodeAutomation.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\CodeAutomation2.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\CodePrepareToInstall.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\UninstallCodeExample1.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\MyDll.dll"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\MyDll\C\MyDll.c"; DestDir: "{app}\Examples\MyDll\C"; Flags: ignoreversion
Source: "isfiles\Examples\MyDll\C\MyDll.def"; DestDir: "{app}\Examples\MyDll\C"; Flags: ignoreversion
Source: "isfiles\Examples\MyDll\C\MyDll.dsp"; DestDir: "{app}\Examples\MyDll\C"; Flags: ignoreversion
Source: "isfiles\Examples\MyDll\Delphi\MyDll.dpr"; DestDir: "{app}\Examples\MyDll\Delphi"; Flags: ignoreversion
Source: "isfiles\Examples\ISPPExample1.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "isfiles\Examples\ISPPExample1License.txt"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "Setup.iss"; DestDir: "{app}\Examples"; Flags: ignoreversion
Source: "Setup.ico"; DestDir: "{app}\Examples"; Flags: ignoreversion
;external files
Source: "{tmp}\ISCrypt.dll"; DestDir: "{app}"; Flags: external ignoreversion; Check: ISCryptCheck
Source: "{srcexe}"; DestDir: "{app}"; DestName: "Ispack-setup.exe"; Flags: external ignoreversion; Check: not ModifyingCheck

[InstallDelete]
;optional ISPP files
Type: files; Name: {app}\Iscmplr.dls
Type: files; Name: {app}\Builtins.iss
;optional ISCrypt files
Type: files; Name: {app}\IsCrypt.dll
;optional desktop icon files
Type: files; Name: {commondesktop}\Inno Setup Compiler.lnk
;older versions created the desktop icon under {userdesktop}
Type: files; Name: "{userdesktop}\Inno Setup Compiler.lnk"

[UninstallDelete]
Type: files; Name: "{app}\Examples\Output\setup.exe"
Type: files; Name: "{app}\Examples\Output\setup-*.bin"
Type: dirifempty; Name: "{app}\Examples\Output"
Type: dirifempty; Name: "{app}\Examples\MyDll\Delphi"
Type: dirifempty; Name: "{app}\Examples\MyDll\C"
Type: dirifempty; Name: "{app}\Examples\MyDll"
Type: dirifempty; Name: "{app}\Examples"

[Icons]
Name: "{group}\Inno Setup Compiler"; Filename: "{app}\Compil32.exe"; WorkingDir: "{app}"; AppUserModelID: "JR.InnoSetup.IDE.5"
Name: "{group}\Inno Setup Documentation"; Filename: "{app}\ISetup.chm";
Name: "{group}\Inno Setup Example Scripts"; Filename: "{app}\Examples\";
Name: "{group}\Inno Setup Preprocessor Documentation"; Filename: "{app}\ISPP.chm";
Name: "{group}\Inno Setup FAQ"; Filename: "{app}\isfaq.htm";
Name: "{group}\Inno Setup Revision History"; Filename: "{app}\whatsnew.htm";
Name: "{commondesktop}\Inno Setup Compiler"; Filename: "{app}\Compil32.exe"; WorkingDir: "{app}"; AppUserModelID: "JR.InnoSetup.IDE.5"; Tasks: desktopicon; Check: not IDECheck

[Run]
Filename: "{tmp}\ide-setup.exe"; StatusMsg: "Installing InnoIDE..."; Parameters: "/verysilent /group=""{groupname}\InnoIDE"" /tasks=""desktopicon,file_association"""; Flags: skipifdoesntexist; Check: IDECheck; Tasks: desktopicon
Filename: "{tmp}\ide-setup.exe"; StatusMsg: "Installing InnoIDE..."; Parameters: "/verysilent /group=""{groupname}\InnoIDE"" /tasks=""!desktopicon,file_association"""; Flags: skipifdoesntexist; Check: IDECheck; Tasks: not desktopicon
Filename: "{app}\Compil32.exe"; Parameters: "/ASSOC"; StatusMsg: "{cm:AssocingFileExtension,Inno Setup,.iss}"; Check: not IDECheck
Filename: "{app}\Compil32.exe"; WorkingDir: "{app}"; Description: "{cm:LaunchProgram,Inno Setup}"; Flags: nowait postinstall skipifsilent; Check: not IDECheck and not ModifyingCheck
Filename: "{code:GetIDEPath}\InnoIDE.exe"; WorkingDir: "{code:GetIDEPath}"; Description: "{cm:LaunchProgram,InnoIDE}"; Flags: nowait postinstall skipifsilent skipifdoesntexist; Check: IDECheck and not ModifyingCheck

[UninstallRun]
Filename: "{app}\Compil32.exe"; Parameters: "/UNASSOC"; RunOnceId: "RemoveISSAssoc"

[Code]
var
  Modifying: Boolean;

  IDEPage, ISPPPage, ISCryptPage: TWizardPage;
  IDECheckBox, ISPPCheckBox, ISCryptCheckBox: TCheckBox;
  IDEOrg: Boolean;

  FilesDownloaded: Boolean;
  
  IDEPath: String;
  IDEPathRead: Boolean;

procedure isxdl_AddFile(URL, Filename: AnsiString);
external 'isxdl_AddFile@files:isxdl.dll stdcall';
function isxdl_DownloadFiles(hWnd: Integer): Integer;
external 'isxdl_DownloadFiles@files:isxdl.dll stdcall';
function isxdl_SetOption(Option, Value: AnsiString): Integer;
external 'isxdl_SetOption@files:isxdl.dll stdcall';

function GetModuleHandle(lpModuleName: LongInt): LongInt;
external 'GetModuleHandleA@kernel32.dll stdcall';
function ExtractIcon(hInst: LongInt; lpszExeFileName: AnsiString; nIconIndex: LongInt): LongInt;
external 'ExtractIconA@shell32.dll stdcall';
function DrawIconEx(hdc: LongInt; xLeft, yTop: Integer; hIcon: LongInt; cxWidth, cyWidth: Integer; istepIfAniCur: LongInt; hbrFlickerFreeDraw, diFlags: LongInt): LongInt;
external 'DrawIconEx@user32.dll stdcall';
function DestroyIcon(hIcon: LongInt): LongInt;
external 'DestroyIcon@user32.dll stdcall';

const
  DI_NORMAL = 3;

function InitializeSetup(): Boolean;
begin
  Modifying := ExpandConstant('{param:modify|0}') = '1';
  FilesDownloaded := False;
  IDEPathRead := False;
    
  Result := True;
end;

function CreateCustomOptionPage(AAfterId: Integer; ACaption, ASubCaption, AIconFileName, ALabel1Caption, ALabel2Caption,
  ACheckCaption: String; var CheckBox: TCheckBox): TWizardPage;
var
  Page: TWizardPage;
  Rect: TRect;
  hIcon: LongInt;
  Label1, Label2: TNewStaticText;
begin
  Page := CreateCustomPage(AAfterID, ACaption, ASubCaption);
  
  try
    AIconFileName := ExpandConstant('{tmp}\' + AIconFileName);
    if not FileExists(AIconFileName) then
      ExtractTemporaryFile(ExtractFileName(AIconFileName));

    Rect.Left := 0;
    Rect.Top := 0;
    Rect.Right := 32;
    Rect.Bottom := 32;

    hIcon := ExtractIcon(GetModuleHandle(0), AIconFileName, 0);
    try
      with TBitmapImage.Create(Page) do begin
        with Bitmap do begin
          Width := 32;
          Height := 32;
          Canvas.Brush.Color := WizardForm.Color;
          Canvas.FillRect(Rect);
          DrawIconEx(Canvas.Handle, 0, 0, hIcon, 32, 32, 0, 0, DI_NORMAL);
        end;
        Parent := Page.Surface;
      end;
    finally
      DestroyIcon(hIcon);
    end;
  except
  end;

  Label1 := TNewStaticText.Create(Page);
  with Label1 do begin
    AutoSize := False;
    Left := WizardForm.SelectDirLabel.Left;
    Width := Page.SurfaceWidth - Left;
    WordWrap := True;
    Caption := ALabel1Caption;
    Parent := Page.Surface;
  end;
  WizardForm.AdjustLabelHeight(Label1);

  Label2 := TNewStaticText.Create(Page);
  with Label2 do begin
    Top := Label1.Top + Label1.Height + ScaleY(12);
    Caption := ALabel2Caption;
    Parent := Page.Surface;
  end;
  WizardForm.AdjustLabelHeight(Label2);

  CheckBox := TCheckBox.Create(Page);
  with CheckBox do begin
    Top := Label2.Top + Label2.Height + ScaleY(12);
    Width := Page.SurfaceWidth;
    Caption := ACheckCaption;
    Parent := Page.Surface;
  end;
  
  Result := Page;
end;

procedure CreateCustomPages;
var
  Caption, SubCaption1, IconFileName, Label1Caption, Label2Caption, CheckCaption: String;
begin
  Caption := 'InnoIDE';
  SubCaption1 := 'Would you like to download and install InnoIDE?';
  IconFileName := 'IDE.ico';
  Label1Caption :=
    'InnoIDE is an easy to use Inno Setup Script editor by Graham Murt and meant as a replacement of the standard' +
    ' Compiler IDE that comes with Inno Setup. See http://www.innoide.org/ for more information.' + #13#10#13#10 +
    'Using InnoIDE is especially recommended for new users.';
  Label2Caption := 'Select whether you would like to download and install InnoIDE, then click Next.';
  CheckCaption := '&Download and install InnoIDE';

  IDEPage := CreateCustomOptionPage(wpSelectProgramGroup, Caption, SubCaption1, IconFileName, Label1Caption, Label2Caption, CheckCaption, IDECheckBox);

  Caption := 'Inno Setup Preprocessor';
  SubCaption1 := 'Would you like to install Inno Setup Preprocessor?';
  IconFileName := 'ISPP.ico';
  Label1Caption :=
    'Inno Setup Preprocessor (ISPP) is an add-on by Alex Yackimoff for Inno Setup. ISPP allows' +
    ' you to conditionally compile parts of scripts, to use compile time variables in your scripts and to use built-in' +
    ' functions which for example can read from the registry or INI files at compile time.' + #13#10#13#10 +
    'ISPP also contains a special version of the ISCC command line compiler which can take variable definitions as command' +
    ' line parameters and use them during compilation.' + #13#10#13#10 +
    'ISPP is compatible with InnoIDE.';
  Label2Caption := 'Select whether you would like to install ISPP, then click Next.';
  CheckCaption := '&Install Inno Setup Preprocessor';

  ISPPPage := CreateCustomOptionPage(IDEPage.ID, Caption, SubCaption1, IconFileName, Label1Caption, Label2Caption, CheckCaption, ISPPCheckBox);

  Caption := 'Encryption Support';
  SubCaption1 := 'Would you like to download encryption support?';
  IconFileName := 'ISCrypt.ico';
  Label1Caption :=
    'Inno Setup supports encryption. However, because of encryption import/export laws in some countries, encryption support is not included in the main' +
    ' Inno Setup installer. Instead, it can be downloaded from a server located in the Netherlands now.';
  Label2Caption := 'Select whether you would like to download encryption support, then click Next.';
  CheckCaption := '&Download and install encryption support';

  ISCryptPage := CreateCustomOptionPage(ISPPPage.ID, Caption, SubCaption1, IconFileName, Label1Caption, Label2Caption, CheckCaption, ISCryptCheckBox);
end;

procedure InitializeWizard;
begin
  CreateCustomPages;
  
  IDECheckBox.Checked := GetPreviousData('IDE', '1') = '1';
  ISPPCheckBox.Checked := GetPreviousData('ISPP', '1') = '1';
  ISCryptCheckBox.Checked := GetPreviousData('ISCrypt', '1') = '1';

  IDEOrg := IDECheckBox.Checked;
end;

procedure RegisterPreviousData(PreviousDataKey: Integer);
begin
  SetPreviousData(PreviousDataKey, 'IDE', IntToStr(Ord(IDECheckBox.Checked)));
  SetPreviousData(PreviousDataKey, 'ISPP', IntToStr(Ord(ISPPCheckBox.Checked)));
  SetPreviousData(PreviousDataKey, 'ISCrypt', IntToStr(Ord(ISCryptCheckBox.Checked)));
end;

procedure DownloadFiles(IDE, ISCrypt: Boolean);
var
  hWnd: Integer;
  URL, FileName: String;
begin
  isxdl_SetOption('label', 'Downloading extra files');
  isxdl_SetOption('description', 'Please wait while Setup is downloading extra files to your computer.');

  try
    FileName := ExpandConstant('{tmp}\WizModernSmallImage-IS.bmp');
    if not FileExists(FileName) then
      ExtractTemporaryFile(ExtractFileName(FileName));
    isxdl_SetOption('smallwizardimage', FileName);
  except
  end;

  //turn off isxdl resume so it won't leave partially downloaded files behind
  //resuming wouldn't help anyway since we're going to download to {tmp}
  isxdl_SetOption('resume', 'false');

  hWnd := StrToInt(ExpandConstant('{wizardhwnd}'));
  
  if IDE then begin
    URL := 'http://www.jrsoftware.org/download.php/innoide.exe';
    FileName := ExpandConstant('{tmp}\ide-setup.exe');
    isxdl_AddFile(URL, FileName);
  end;
  
  if ISCrypt then begin
    URL := 'http://www.jrsoftware.org/download.php/iscrypt.dll';
    FileName := ExpandConstant('{tmp}\ISCrypt.dll');
    isxdl_AddFile(URL, FileName);
  end;

  if isxdl_DownloadFiles(hWnd) <> 0 then
    FilesDownloaded := True
  else
    SuppressibleMsgBox('Setup could not download the extra files. Try again later or download and install the extra files manually.' + #13#13 + 'Setup will now continue installing normally.', mbError, mb_Ok, idOk);
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  if IDECheckBox.Checked or ISCryptCheckBox.Checked then
    DownloadFiles(IDECheckBox.Checked, ISCryptCheckBox.Checked);
  Result := '';
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
  Result := Modifying and ((PageID = wpSelectDir) or (PageID = wpSelectProgramGroup) or ((PageID = IDEPage.ID) and IDEOrg));
end;

function ModifyingCheck: Boolean;
begin
  Result := Modifying;
end;

function IDECheck: Boolean;
begin
  Result := IDECheckBox.Checked and FilesDownloaded;
end;

function ISPPCheck: Boolean;
begin
  Result := ISPPCheckBox.Checked;
end;

function ISCryptCheck: Boolean;
begin
  Result := ISCryptCheckBox.Checked and FilesDownloaded;
end;

function GetIDEPath(S: String): String;
var
  IDEPathKeyName, IDEPathValueName: String;
begin
  if not IDEPathRead then begin
    IDEPathKeyName := 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{1E8BAA74-62A9-421D-A61F-164C7C3943E9}_is1';
    IDEPathValueName := 'Inno Setup: App Path';

    if not RegQueryStringValue(HKLM, IDEPathKeyName, IDEPathValueName, IDEPath) then begin
      if not RegQueryStringValue(HKCU, IDEPathKeyName, IDEPathValueName, IDEPath) then begin
        SuppressibleMsgBox('Error launching InnoIDE:'#13'Could not read InnoIDE path from registry.', mbError, mb_Ok, idOk);
        IDEPath := '';
      end;
    end;

    IDEPathRead := True;
  end;

  Result := IDEPath;
end;
