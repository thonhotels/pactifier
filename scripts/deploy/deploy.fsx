#r "paket:
    nuget Fake.Core.Target
    nuget Fake.DotNet.Cli 
    nuget Fake.Tools.Git //"

#load "./versionNumber.fsx"
#load ".fake/deploy.fsx/intellisense.fsx"

#if !FAKE
  #r "netstandard"
#endif

open Fake.Core
open Fake.DotNet

let artifacts = __SOURCE_DIRECTORY__ + "/artifacts"

Target.create "Nuget-push" (fun _ ->     
    let versionNumber = VersionNumber.getFromGit ()

    match versionNumber with
    | Some x -> 
        DotNet.exec 
            id 
            "push" <| sprintf "%s/*nupkg --source https://api.nuget.org/v3/index.json --no-service-endpoint --api-key $(nuget-apikey)" artifacts
            |> ignore
    | None -> ()
)

"Nuget-push"

Target.runOrDefaultWithArguments "Nuget-push"