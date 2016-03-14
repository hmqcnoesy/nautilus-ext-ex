# nautilus-ext-ex

```shell
c:
echo off
cls 

rem get a ref to the nautilus install location (could be program files or x86)
SET NautPath=%ProgramFiles%\Thermo\Nautilus
IF NOT EXIST "%NautPath%" SET NautPath=%programfiles(x86)%\thermo\nautilus
echo Using Nautilus path = %NautPath%

rem copy the .NET extensions into the Nautilus application folder
rem cd "c:\program files\thermo\nautilus"
cd %NautPath%
copy \\nautilus\extensions\*.dll
copy \\nautilus\extensions\*.xml
copy \\nautilus\extensions\NautilusExtensions.dll.config Nautilus.exe.config

rem register the .NET extensions using regasm, and create a tlb file for Nautilus interop.
c:\windows\microsoft.net\framework\v4.0.30319\regasm.exe /tlb "%NautPath%\nautilusextensions.dll"

pause
```
