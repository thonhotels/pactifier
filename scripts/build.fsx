#r "paket:
    nuget Fake.Core.Target
    nuget Fake.Core.Globbing
    nuget Fake.Core.Trace
    nuget Fake.DotNet.Cli //"

#load "deploy/versionNumber.fsx"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.Core.Globbing.Operators

let projectFolder = "core"
let project = sprintf "../src/%s" projectFolder

let artifacts = __SOURCE_DIRECTORY__ + "/deploy/artifacts"

Target.create "Install-dotnet" (fun _ ->
        DotNet.install DotNet.Versions.FromGlobalJson |> ignore
)

Target.create "Clean" (fun _ ->
        DotNet.exec id "clean" |> ignore
        Shell.cleanDirs [artifacts;]
    )

Target.create "Restore-packages" (fun _ ->       
        [project;]
        |> Seq.iter (DotNet.restore id)    
    )
  
Target.create "Build" (fun _ ->
        [project;]
        |> Seq.iter (DotNet.build id)    
    )   

Target.create "Test" (fun _ ->
        !! "../tests/**/*.csproj" 
        |> Seq.filter(fun p -> 
                        not ([]
                        |> List.exists (fun v -> p.Contains(v)))) 
        |> Seq.iter (DotNet.test id)
    )         

Target.create "Pack" (fun _ -> 
    let versionNumber = VersionNumber.getFromGit ()

    match versionNumber with
    | Some x -> 
        Trace.log ("version number was something, doing pack")
        Shell.replaceInFiles 
                [("$(BUILD_BUILDNUMBER)", x)]  
                [sprintf "../Directory.Build.props" ]
        DotNet.pack 
            (fun o -> 
                { o with 
                    OutputPath = Some artifacts 
                }) 
            project
    | None -> Trace.log ("Latest commit has no tag, no nuget created")    
)

"Install-dotnet"
==> "Clean"
==> "Restore-packages"
==> "Build"
==> "Test"
==> "Pack"

Target.runOrDefaultWithArguments "Pack"