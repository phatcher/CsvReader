/// FAKE Build script

#r "packages/build/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open Fake.Git
open Fake.ReleaseNotesHelper
open Fake.Testing.NUnit3
open System.IO

// Version info
let projectName = "LumenWorks.Framework.IO"
let authors = ["Sébastien Lorion"; "Paul Hatcher"]
let copyright = "Copyright © 2005 Sébastien Lorion, 2014 Paul Hatcher"

let release = LoadReleaseNotes "RELEASE_NOTES.md"

// Properties
let buildDir = "./build"
let toolsDir = getBuildParamOrDefault "tools" "packages/build"
let solutionFile = "CsvReader.sln"

let nunitPath = toolsDir @@ "/NUnit.ConsoleRunner/tools/nunit3-console.exe"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir;]
)

Target "PackageRestore" (fun _ ->
    !! solutionFile
    |> MSBuildRelease "" "Restore"
    |> Log "AppBuild-Output: "
)

Target "SetVersion" (fun _ ->
    let commitHash = 
        try 
            Information.getCurrentHash()
        with
            | ex -> printfn "Exception! (%s)" (ex.Message); ""
    let infoVersion = String.concat " " [release.AssemblyVersion; commitHash]
    CreateCSharpAssemblyInfo "./code/SolutionInfo.cs"
        [Attribute.Product projectName
         Attribute.Copyright copyright
         Attribute.Version release.AssemblyVersion
         Attribute.FileVersion release.AssemblyVersion
         Attribute.InformationalVersion infoVersion]
)

Target "Build" (fun _ ->
    !! solutionFile
    |> MSBuild "" "Build"
        [
            "Configuration", "Release"
            "Platform", "Any CPU"
            "Authors", authors |> String.concat ", "
            "PackageVersion", release.AssemblyVersion
            "PackageReleaseNotes", release.Notes |> toLines
            "IncludeSymbols", "true"
        ]
    |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    Directory.GetFiles(buildDir, "*.Tests.Unit.dll", SearchOption.AllDirectories)
    // Filter out the NET Core versions as the NUnit runner can't execute them
    |> Array.filter (fun x -> x.Contains("netcoreapp") = false)
    |> NUnit3 (fun p ->
       {p with
          ToolPath = nunitPath
          // Oddity as this creates a build directory in the build directory
          //WorkingDir = buildDir
          ShadowCopy = false})
)

Target "Release" (fun _ ->
    let tag = String.concat "" ["v"; release.AssemblyVersion] 
    Branches.tag "" tag
    Branches.pushTag "" "origin" tag
)

Target "Default" DoNothing

// Dependencies
"Clean"
    ==> "SetVersion"
    ==> "PackageRestore"
    ==> "Build"
    ==> "Test"
    ==> "Default"
    ==> "Release"

RunTargetOrDefault "Default"