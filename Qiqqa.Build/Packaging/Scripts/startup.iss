[Code]

// =======================================
// Version Checking
// =======================================
function GetNumber(var temp: String): Integer;
var
  part: String;
  pos1: Integer;
begin
  if Length(temp) = 0 then
  begin
    Result := -1;
    Exit;
  end;
	pos1 := Pos('.', temp);
	if (pos1 = 0) then
	begin
	  Result := StrToInt(temp);
	temp := '';
	end
	else
	begin
	part := Copy(temp, 1, pos1 - 1);
	  temp := Copy(temp, pos1 + 1, Length(temp));
	  Result := StrToInt(part);
	end;
end;

function CompareInner(var temp1, temp2: String): Integer;
var
  num1, num2: Integer;
begin
	num1 := GetNumber(temp1);
  num2 := GetNumber(temp2);
  if (num1 = -1) or (num2 = -1) then
  begin
    Result := 0;
    Exit;
  end;
  if (num1 > num2) then
  begin
	Result := 1;
  end
  else if (num1 < num2) then
  begin
	Result := -1;
  end
  else
  begin
	Result := CompareInner(temp1, temp2);
  end;
end;

function CompareVersion(str1, str2: String): Integer;
var
  temp1, temp2: String;
begin
	temp1 := str1;
	temp2 := str2;
	Result := CompareInner(temp1, temp2);
end;

// =======================================
// Already Running?
// =======================================
// function IsModuleLoaded to call at install time
// added also setuponly flag
function IsModuleLoaded(modulename: String ):  Boolean;
external 'IsModuleLoaded@files:psvince.dll stdcall setuponly';

// function IsModuleLoadedU to call at uninstall time
// added also uninstallonly flag
function IsModuleLoadedU(modulename: String ):  Boolean;
external 'IsModuleLoaded@{app}\psvince.dll stdcall uninstallonly';

function IsAppRunning(modulename: String): Boolean;
begin
  Result := IsModuleLoaded( modulename );
end;

// =======================================
// Testing if under Windows safe mode
// =======================================
function GetSystemMetrics( define: Integer ): Integer; external 'GetSystemMetrics@user32.dll stdcall';
Const SM_CLEANBOOT = 67;

function IsSafeModeBoot(): Boolean;
begin
  // 0 = normal boot, 1 = safe mode, 2 = safe mode with networking
 Result := ( GetSystemMetrics( SM_CLEANBOOT ) <> 0 );
end;
