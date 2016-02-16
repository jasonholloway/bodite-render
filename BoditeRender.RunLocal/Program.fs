open BoditeRender

open System.IO;
open System.Security.AccessControl

[<EntryPoint>]
let main argv =     

    let d = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "BoditeTest"))


    let resolver = FSResolver.create (Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\..\..\BoditeRender.Templates"))

    let committer = FSCommitter.create (d.FullName)
        
            
    CouchDbLoader.loadDbModel "http://localhost:5984/bb"
    |> Hydrate.hydrateModel
    |> Pages.buildPages
    |> Renderer(resolver).renderPages
    |> Seq.iter (fun f -> committer f)
        
    printfn "DONE!"

    d.Delete(true)

    0
