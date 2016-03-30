open BoditeRender

open System.IO;
open System.Security.AccessControl

[<EntryPoint>]
let main argv =     
    let templatePath = argv.[0]
    //let outputPath = argv.[1]

    let templateLoader = new FSLoader(templatePath)
    let fileCommitter = new S3Committer()
                    
    CouchDbLoader.loadDbModel "http://localhost:5984/bb"
    |> Hydrate.hydrateModel
    |> (fun m ->
            let pages = m |> Pages.buildPages

            let pageReg = pages |> Pages.buildPageRegistry

            let ctx = RenderContext(m, (fun objs -> objs 
                                                     |> Seq.map (fun o -> PageKey(o))
                                                     |> Set.ofSeq
                                                     |> pageReg.TryFind)
                                                     )            
            pages
            |> Renderer(templateLoader, ctx).renderPages
            |> Seq.iter fileCommitter.Commit
            ) 
        
    printfn "Rendered to S3"

    0
