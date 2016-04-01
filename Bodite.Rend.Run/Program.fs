open BoditeRender

open System.IO;
open System.Security.AccessControl
open Amazon.S3

[<EntryPoint>]
let main argv =     
    let templatePath = argv.[0]
    //let outputPath = argv.[1]
        
    use s3Client =     
        let s3Config = new AmazonS3Config()
        s3Config.ServiceURL <- "http://localhost:9988"
        s3Config.UseHttp <- true
        s3Config.ReadEntireResponse <- true
        s3Config.ForcePathStyle <- true
        
        let creds = Amazon.Runtime.BasicAWSCredentials("", "")
        
        new AmazonS3Client(creds, s3Config)




    let templateLoader = new FSLoader(templatePath)
    let fileCommitter = new S3Committer(s3Client, "bodite")
                    
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
