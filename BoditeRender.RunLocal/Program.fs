open BoditeRender

[<EntryPoint>]
let main argv =     
    let resolver = FSResolver.create (System.IO.Directory.GetCurrentDirectory())

    let committer = FSCommitter.create (System.IO.Directory.GetCurrentDirectory())
        
    Hydrate.hydrateModel()
    |> Pages.buildPages
    |> Renderer(resolver).renderPages
    |> Seq.iter (fun f -> committer f)
        
    printfn "DONE!"
    0
