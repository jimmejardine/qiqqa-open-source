[CustomMessages]
msi45win60_title=Windows Installer 4.5 for Windows Vista & Windows 7
msi45win52_title=Windows Installer 4.5 for Server 2003
msi45win51_title=Windows Installer 4.5 for Windows XP

msi45win60_size=1.7 MB
msi45win52_size=3.0 MB
msi45win51_size=3.2 MB



[Code]
const
	msi45win60_url = 'http://download.microsoft.com/download/2/6/1/261fca42-22c0-4f91-9451-0e0f2e08356d/Windows6.0-KB942288-v2-x86.msu';
  msi45win52_url = 'http://download.microsoft.com/download/2/6/1/261fca42-22c0-4f91-9451-0e0f2e08356d/WindowsServer2003-KB942288-v4-x86.exe';
  msi45win51_url = 'http://download.microsoft.com/download/2/6/1/261fca42-22c0-4f91-9451-0e0f2e08356d/WindowsXP-KB942288-v3-x86.exe';
procedure msi45(MinVersion: string);
begin
	// Check for required Windows Installer 3.0 on Windows 2000 or higher
	if minwinversion(6, 0) and (fileversion(ExpandConstant('{sys}{\}msi.dll')) < MinVersion) then
		AddProduct('msi45win60.msu',
			'/quiet /norestart',
			CustomMessage('msi45win60_title'),
			CustomMessage('msi45win60_size'),
			msi45win60_url,false,false)
	else if minwinversion(5, 2) and (fileversion(ExpandConstant('{sys}{\}msi.dll')) < MinVersion) then
		AddProduct('msi45win52.exe',
			'/quiet /norestart',
			CustomMessage('msi45win52_title'),
			CustomMessage('msi45win52_size'),
			msi45win52_url,false,false)
	else if minwinversion(5, 1) and (fileversion(ExpandConstant('{sys}{\}msi.dll')) < MinVersion) then
		AddProduct('msi45win51.exe',
			'/quiet /norestart',
			CustomMessage('msi45win51_title'),
			CustomMessage('msi45win51_size'),
			msi45win51_url,false,false);
end;