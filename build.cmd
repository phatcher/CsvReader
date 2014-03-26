@echo off
if not exist code\packages\FAKE\tools\Fake.exe ( 
  nuget\nuget.exe install FAKE -OutputDirectory code\packages -ExcludeVersion
)
nuget\nuget.exe restore
code\packages\FAKE\tools\FAKE.exe build.fsx %*
pause