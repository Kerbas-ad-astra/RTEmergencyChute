
copy RTEmergencyChute.dll Output\Plugins\RTEmergencyChute.dll

set DEST="C:\KSP_DEV\GameData\RTEmergencyChute"
xcopy Output %DEST% /D /E /C /R /I /K /Y

call C:\KSP_DEV\KSP.EXE