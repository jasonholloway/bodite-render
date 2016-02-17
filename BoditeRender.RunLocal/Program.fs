open BoditeRender

open System.IO;
open System.Security.AccessControl

[<EntryPoint>]
let main argv =     
    let templatePath = argv.[0]
    let outputPath = argv.[1]

    let resolver = FSResolver.create templatePath
    let committer = FSCommitter.create outputPath
                    
    CouchDbLoader.loadDbModel "http://localhost:5984/bb"
    |> Hydrate.hydrateModel
    |> (fun m ->
            m
            |> Pages.buildPages
            |> Renderer(resolver).renderPages (RenderContext(m, (fun r -> HomePage(Locales.LV) :> Page)))
            |> Seq.iter (fun f -> committer f)
            ) 
        
    printfn "Rendered to %s" outputPath

    0
