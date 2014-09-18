@echo off
"C:\Program Files (x86)\Inno Setup 5\iscc.exe" "/ssigntool=C:\Program Files (x86)\Windows Kits\8.0\bin\x64\signtool.exe sign /t http://timestamp.digicert.com /f C:\Work\Cert\MySPC.pfx $f" "Script.iss"
if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   "C:\Program Files (x86)\Inno Script Studio\ISStudio.exe" "Script.iss"
)