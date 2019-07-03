// requires Windows 2000 Service Pack 4, Windows Server 2003 Service Pack 1, Windows XP Service Pack 2
// SQL Express 2005 Service Pack 1+ should be installed for SQL Express 2005 to work on Vista
// requires windows installer 3.1
// http://www.microsoft.com/downloads/details.aspx?FamilyID=220549b5-0b07-4448-8848-dcc397514b41

[CustomMessages]
sql2005expressx86_title=SQL Server 2005 Express SP3 x86
sql2005expressx64_title=SQL Server 2005 Express SP3 x64

en.sql2005expressx86_size=38.1 MB
de.sql2005expressx86_size=38,1 MB
en.sql2005expressx64_size=58.1 MB
de.sql2005expressx64_size=58,1 MB


[Code]
const
//	sql2005express_url = 'http://download.microsoft.com/download/f/1/0/f10c4f60-630e-4153-bd53-c3010e4c513b/SQLEXPR.EXE';
	sql2005expressx86_url = 'http://download.microsoft.com/download/4/B/E/4BED5810-C8C0-4697-BDC3-DBC114B8FF6D/SQLEXPR32_NLA.EXE';
	sql2005expressx64_url = 'http://download.microsoft.com/download/4/B/E/4BED5810-C8C0-4697-BDC3-DBC114B8FF6D/SQLEXPR_NLA.EXE';

procedure sql2005express();
var
	version: string;
begin
	//CHECK NOT FINISHED YET
	//RTM: 9.00.1399.06
	//Service Pack 1: 9.1.2047.00
	//Service Pack 2: 9.2.3042.00
    // Newer detection method required for SP3 and x64
	//Service Pack 3: 9.00.4035.00
	//RegQueryDWordValue(HKLM, 'Software\Microsoft\Microsoft SQL Server\90\DTS\Setup', 'Install', version);
	RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Microsoft SQL Server\SQLEXPRESS\MSSQLServer\CurrentVersion', 'CurrentVersion', version);
	if version < '9.00.4035' then begin
        if isX64 then
    		AddProduct('sql2005express.exe',
    			'/qb ADDLOCAL=ALL INSTANCENAME=SQLEXPRESS',
    			CustomMessage('sql2005expressx64_title'),
    			CustomMessage('sql2005expressx64_size'),
    			sql2005expressx64_url,false,false)
        else
    		AddProduct('sql2005express.exe',
    			'/qb ADDLOCAL=ALL INSTANCENAME=SQLEXPRESS',
    			CustomMessage('sql2005expressx86_title'),
    			CustomMessage('sql2005expressx86_size'),
    			sql2005expressx86_url,false,false);
    end;
end;
