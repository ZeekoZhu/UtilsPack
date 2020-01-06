open Fake.IO

#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.MyFakeTools

///========================================
/// Utils
///========================================

let MyFeed = "https://www.myget.org/F/zeekoget/api/v2/package"

let packages =
    !! "./ExpressionCache/*.csproj"
    ++ "./Zeeko.BaseDevel.DependencyInjection/*.csproj"
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
        Directory.delete "publish"
        let packOpt (opt : Paket.PaketPackParams) =
            { opt with
                OutputPath = "publish"
            }
        Paket.pack packOpt
    )

Target.create "push"
    ( fun _ ->
        Paket.push ( fun opt ->
            { opt with
                WorkingDir = "publish"
                PublishUrl = MyFeed
            })
    )

"build"
    ==> "pack"
    ==> "push"

"build"
    ==> "test"
Target.runOrDefault "test"
