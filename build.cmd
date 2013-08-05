@echo off

IF %processor_architecture% == x86 goto 32bit

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe /target:Build build\msbuild.xml
goto after

:32bit
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /target:Build build\msbuild.xml

:after

if %ERRORLEVEL% NEQ 0 goto errors

goto finish

:errors
pause

:finish