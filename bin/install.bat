@ECHO OFF
net session >nul 2>&1
IF NOT %ERRORLEVEL% EQU 0 (
   ECHO ERROR: Please run Bat as Administrator.
   PAUSE
   EXIT /B 1
)
@SETLOCAL enableextensions
@CD /d "%~dp0"
REM The following directory is for .NET 4
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%
ECHO Start Installing WindowsService_tutorial...
ECHO ---------------------------------------------------
InstallUtil /i Debug\WindowsService_tutorial.exe
ECHO ---------------------------------------------------
ECHO Done.
PAUSE