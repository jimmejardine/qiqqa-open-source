// IDP: This plugin is capable of downloading any file (zip or any format).
// But you need to use the unzip tool to extract the file after downloading.
//
// Following script gives you an example on how to use the unzip functionality.
//
// -------------------------------------------------------------------------
//
//	Unzip Helper for executing 7zip without blocking the InnoSetup GUI
//
//  ----
//
//	The main procedure is the non-blocking Unzip().
//	Your GUI will remain responsive during the unzip operation.
//
//  Written by Rik and Jens A. Koch (@jakoch) on StackOverflow:
//  http://stackoverflow.com/questions/32256432/how-to-execute-7zip-without-blocking-the-innosetup-ui
//
//  ----
//
//  Usage:
//
//	1. Include this ISS with
//
//	   // #include "..\some\where\unzip.iss"
//
//	2. Add the unzip tool "7za.exe" to the [Files] section of your installer
//	   and copy it to the temp folder during installation.
//
//     // [Files]
//     // Source: ..\some\where\7za.exe; DestDir: {tmp}; Flags: dontcopy
//
//	3. Finally, extract your files using Unzip(source, target); in the [Code] section.
//

#IFDEF UNICODE
  #DEFINE AW "W"
#ELSE
  #DEFINE AW "A"
#ENDIF

// --- Start "ShellExecuteEx" Helper

const
  WAIT_TIMEOUT = $00000102;
  SEE_MASK_NOCLOSEPROCESS = $00000040;
  INFINITE = $FFFFFFFF;     { Infinite timeout }

type
  TShellExecuteInfo = record
    cbSize: DWORD;
    fMask: Cardinal;
    Wnd: HWND;
    lpVerb: string;
    lpFile: string;
    lpParameters: string;
    lpDirectory: string;
    nShow: Integer;
    hInstApp: THandle;
    lpIDList: DWORD;
    lpClass: string;
    hkeyClass: THandle;
    dwHotKey: DWORD;
    hMonitor: THandle;
    hProcess: THandle;
  end;

function ShellExecuteEx(var lpExecInfo: TShellExecuteInfo): BOOL;
  external 'ShellExecuteEx{#AW}@shell32.dll stdcall';
function WaitForSingleObject(hHandle: THandle; dwMilliseconds: DWORD): DWORD;
  external 'WaitForSingleObject@kernel32.dll stdcall';
function CloseHandle(hObject: THandle): BOOL; external 'CloseHandle@kernel32.dll stdcall';

// --- End "ShellExecuteEx" Helper

// --- Start "Application.ProcessMessage" Helper
{
   InnoSetup does not provide Application.ProcessMessage().
   This is "generic" code to recreate a "Application.ProcessMessages"-ish procedure,
   using the WinAPI function PeekMessage(), TranslateMessage() and DispatchMessage().
}
type
  TMsg = record
    hwnd: HWND;
    message: UINT;
    wParam: Longint;
    lParam: Longint;
    time: DWORD;
    pt: TPoint;
  end;

const
  PM_REMOVE = 1;

function PeekMessage(var lpMsg: TMsg; hWnd: HWND; wMsgFilterMin, wMsgFilterMax, wRemoveMsg: UINT): BOOL; external 'PeekMessageA@user32.dll stdcall';
function TranslateMessage(const lpMsg: TMsg): BOOL; external 'TranslateMessage@user32.dll stdcall';
function DispatchMessage(const lpMsg: TMsg): Longint; external 'DispatchMessageA@user32.dll stdcall';

procedure AppProcessMessage;
var
  Msg: TMsg;
begin
  while PeekMessage(Msg, WizardForm.Handle, 0, 0, PM_REMOVE) do 
  begin
    TranslateMessage(Msg);
    DispatchMessage(Msg);
  end;
end;

// --- End "Application.ProcessMessage" Helper

procedure Unzip(source: String; targetdir: String);
var
  unzipTool, unzipParams : String; // path and param for the unzip tool
  ExecInfo: TShellExecuteInfo;     // info object for ShellExecuteEx()
begin
    // source and targetdir might contain {tmp} or {app} constant, so expand/resolve it to path names
    source := ExpandConstant(source);
    targetdir := ExpandConstant(targetdir);

    // prepare 7zip execution
    unzipTool := ExpandConstant('{tmp}\7za.exe');
    unzipParams := ' x "' + source + '" -o"' + targetdir + '" -y';

    // prepare information about the application being executed by ShellExecuteEx()
    ExecInfo.cbSize := SizeOf(ExecInfo);
    ExecInfo.fMask := SEE_MASK_NOCLOSEPROCESS;
    ExecInfo.Wnd := 0;
    ExecInfo.lpFile := unzipTool;
    ExecInfo.lpParameters := unzipParams;
    ExecInfo.nShow := SW_HIDE;

    if not FileExists(unzipTool) then 
	    MsgBox('UnzipTool not found: ' + unzipTool, mbError, MB_OK)
    else if not FileExists(source) then 
		MsgBox('File was not found while trying to unzip: ' + source, mbError, MB_OK)
    else 
	begin
          {
             The unzip tool is executed via ShellExecuteEx()
             Then the installer uses a while loop with the condition
             WaitForSingleObject and a very minimal timeout
             to execute AppProcessMessage.

             AppProcessMessage is itself a helper function, because
             Innosetup does not provide Application.ProcessMessages().
             Its job is to be the message pump to the InnoSetup GUI.

             This trick makes the window responsive/dragable again,
             while the extraction is done in the background.
          }

          if ShellExecuteEx(ExecInfo) then
          begin
            while WaitForSingleObject(ExecInfo.hProcess, 100) = WAIT_TIMEOUT do 
			begin
                AppProcessMessage;
                WizardForm.Refresh();
            end;
            CloseHandle(ExecInfo.hProcess);
          end;
    end;
end;

