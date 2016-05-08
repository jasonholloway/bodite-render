open BoditeRender

open System.IO;
open System.Security.AccessControl

[<EntryPoint>]
let main argv =     
    let templatePath = argv.[0]
    let outputPath = argv.[1]

    let loader = new FSLoader(templatePath)
    let repo = new FSRepo(outputPath)
                    
    CouchDbLoader.loadDbModel "http://localhost:5984/bb"
    |> Hydrate.hydrateModel
    |> (fun m ->
            let pages = m |> Pages.buildPages

            let pageReg = pages |> PageReg.build

            let ctx = RenderContext(m, pageReg |> PageReg.findPage)            
            pages
            |> Renderer(loader, ctx).renderPages
            |> Seq.iter repo.Write
            ) 
        
    printfn "Rendered to %s" outputPath

    0
