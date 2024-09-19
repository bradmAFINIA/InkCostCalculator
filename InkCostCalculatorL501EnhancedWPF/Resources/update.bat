@echo off
setlocal ENABLEDELAYEDEXPANSION
for /f "delims=" %%a in ('call "%~dp0\efm8load.exe" %*') do (
    set str=%%a
    rem Remove spaces. There's a note about findstr not working with strings with spaces.
    set str=!str: =!
    echo !str!|findstr /l "Downloadcomplete"
    if !errorlevel! == 0 (
        echo !str!|findstr /l "Downloadcompletewith[0]errors"
        exit /b !errorlevel!
    )
)
exit /b 1
