@echo off
setlocal DISABLEDELAYEDEXPANSION
set /a out.cnt=err.cnt=0
for /f "delims=: tokens=1*" %%a in ('call "%~dp0\USBSend.exe" "%*" 2^>err.log ^| findstr /n "^"') do (
    set "out.%%a=%%b"
    set "out.cnt=%%a"
)
for /f "delims=: tokens=1*" %%a in ('findstr /n "^" err.log') do (
    set "err.%%a=%%b"
    set "err.cnt=%%a"
)
setlocal ENABLEDELAYEDEXPANSION
for /l %%n in (1 1 %err.cnt%) do (
    set str=!err.%%n!
    rem Remove spaces. There's a note about findstr not working with strings with spaces.
    set str=!str: =!
    echo Parsing error string !str!
    echo !str!|findstr /l "DidnotfindUSBDevice:error0"
    if !errorlevel! == 0 (
        exit /b 2
    )
)

for /l %%n in (1 1 %out.cnt%) do (
    set str=!out.%%n!
    rem Remove spaces. There's a not about findstr not working with strings with spaces.
    set str=!str: =!
    echo Parsing output string !str!
    echo !str!|findstr /l "Filedownloadcomplete"
    if !errorlevel! == 0 (
        exit /b 0
    )
)

exit /b 1