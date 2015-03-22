@echo off
cls

if not exist packages\FAKE\tools\Fake.exe ( 
  nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion
)

SET TARGET="Default"

if NOT [%1]==[] (set TARGET="%1")

"packages\FAKE\tools\FAKE.exe" "build.fsx" "target=%TARGET%" %*
pause