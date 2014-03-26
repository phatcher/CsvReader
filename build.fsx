/// FAKE Build script

#r "code/packages/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let buildDir = "./build"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "Build" (fun _ ->
    !! "code/**/*.csproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: "
)

Target "Default" (fun _ ->
    trace "Hello world"
)

// Dependencies
"Clean"
    ==> "Build"
    ==> "Default"

RunTargetOrDefault "Default"