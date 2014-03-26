@echo off
if not exist packages\FAKE\tools\Fake.exe ( 
  nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion
)
nuget\nuget.exe restore
packages\FAKE\tools\FAKE.exe build.fsx %*
pause