[CustomMessages]
sql2008r2expressx86_title=SQL Server 2008 R2 Express x86 (Including tools)
sql2008r2expressx64_title=SQL Server 2008 R2 Express x64 (Including tools)

//sql2008r2expressx86_size=58.2 MB
sql2008r2expressx86_size=71.0 MB
sql2008r2expressx64_size=74.0 MB


[Code]
const
	sql2008r2expressx86_url = 'http://download.microsoft.com/download/5/1/A/51A153F6-6B08-4F94-A7B2-BA1AD482BC75/SQLEXPR_x86_ENU.exe';
	sql2008r2expressx64_url = 'http://download.microsoft.com/download/5/1/A/51A153F6-6B08-4F94-A7B2-BA1AD482BC75/SQLEXPR_x64_ENU.exe';

procedure sql2008express();
var
	version: string;
begin
	// This check does not take into account that a full version of SQL Server could be installed,
	// making Express unnecessary.
	RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Microsoft SQL Server\SQLEXPRESS\MSSQLServer\CurrentVersion', 'CurrentVersion', version);
	if (version < '10.5') (*or (version > '9.00') or (version = '') *) then begin
        if isX64() then
    		AddProduct('sql2008expressx64.exe',
    			'/QS  /IACCEPTSQLSERVERLICENSETERMS /ACTION=Install /FEATURES=All /INSTANCENAME=SQLEXPRESS /SQLSVCACCOUNT="NT AUTHORITY\Network Service" /SQLSYSADMINACCOUNTS="builtin\administrators"',
    			CustomMessage('sql2008r2expressx64_title'),
    			CustomMessage('sql2008r2expressx64_size'),
    			sql2008r2expressx64_url,false,false)
        else
    		AddProduct('sql2008expressx86.exe',
    			'/QS  /IACCEPTSQLSERVERLICENSETERMS /ACTION=Install /FEATURES=All /INSTANCENAME=SQLEXPRESS /SQLSVCACCOUNT="NT AUTHORITY\Network Service" /SQLSYSADMINACCOUNTS="builtin\administrators"',
    			CustomMessage('sql2008r2expressx86_title'),
    			CustomMessage('sql2008r2expressx86_size'),
    			sql2008r2expressx86_url,false,false);
    end;
end;
