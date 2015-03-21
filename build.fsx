/// FAKE Build script

#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open Fake.NuGetHelper
open Fake.RestorePackageHelper

// Version info
let version = "3.8.3" 

// Properties
let buildDir = "./build"
let toolsDir = "./tools"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "PackageRestore" (fun _ ->
    RestorePackages()
)

Target "SetVersion" (fun _ ->
    CreateCSharpAssemblyInfo "./code/SolutionInfo.cs"
        [Attribute.Version version
         Attribute.FileVersion version]
)

Target "Build" (fun _ ->
    !! "./code/**/*.csproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! (buildDir + "/*.Tests.Unit.dll")
    |> NUnit (fun p ->
       {p with
          ToolPath = toolsDir @@ "NUnit"
          DisableShadowCopy = true
          OutputFile = buildDir @@ "TestResults.xml"})
)

Target "Pack" (fun _ ->
    NuGet (fun p ->
      { p with 
          Version = version
          OutputPath = buildDir}) 
      "nuget/CsvReader.nuspec"
)

// Dependencies
"Clean"
    ==> "SetVersion"
    ==> "PackageRestore"
    ==> "Build"
    ==> "Test"
    ==> "Pack"

RunTargetOrDefault "Test"