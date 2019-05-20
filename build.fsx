#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.MyFakeTools

///========================================
/// Utils
///========================================
let inline withWorkDir wd =
    DotNet.Options.withWorkingDirectory wd

let packages =
    !! "./EFKata/*.csproj"
    ++ "./ExpressionCache/*.csproj"
    ++ "./ZeekoUtilsPack.AspNetCore/*.csproj"
    ++ "./ZeekoUtilsPack.BCLExt/*.csproj"

Target.create "test"
    (fun _ -> DotNet.test id "./Test/Test/Test.csproj")

Target.create "build"
    ( fun _ ->
        packages
        |> Seq.iter (DotNet.build id)
    )

Target.create "pack"
    ( fun _ ->
        Utils.runCmd "paket" ["pack"; "publish"]
    )

"build"
    ==> "pack"

"build"
    ==> "test"
Target.runOrDefault "test"
