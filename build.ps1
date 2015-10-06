[CmdletBinding()]
Param(
    [Parameter(mandatory = $true, position = 0)][string]$Target,
    [Parameter(mandatory = $false, position = 1, ValueFromRemainingArguments=$true)]$Remaining
)

if (-Not (Test-Path packages\FAKE\tools\fake.exe))
{
    nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion
}

. "packages\FAKE\tools\fake.exe" "build.fsx" "target=$Target" @Remaining