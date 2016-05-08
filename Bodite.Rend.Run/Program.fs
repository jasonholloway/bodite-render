module Program

open BoditeRender
open System
open System.IO
open System.Security.AccessControl
open Amazon.S3


let Port = 9988
let BucketName = "bodite"//Guid.NewGuid().ToString()
let BaseUri = UriBuilder("http", "localhost", Port).Uri 



[<EntryPoint>]
let main argv =     
    let templatePath = argv.[0]
    //let outputPath = argv.[1]
        
    use s3Client =     
        let s3Config = new AmazonS3Config()
        s3Config.ServiceURL <- BaseUri.ToString()
        s3Config.UseHttp <- true
        s3Config.ReadEntireResponse <- true
        s3Config.ForcePathStyle <- true
        
        let creds = Amazon.Runtime.BasicAWSCredentials("", "")
        
        try
            new AmazonS3Client(creds, s3Config)
        with
        | x -> failwith "hello!"

        

    use templateLoader = new FSLoader(templatePath)
    use repo = new S3Repo(s3Client, BucketName)

    CouchDbLoader.loadDbModel "http://localhost:5984/bb"
    |> Hydrate.hydrateModel
    |> (fun m ->                        
            let pages = m |> Pages.buildPages

            let pageReg = pages |> PageReg.build

            let ctx = RenderContext(m, pageReg |> PageReg.findPage)
                     
            pages 
            |> Renderer(templateLoader, ctx).renderPages
            |> Seq.iter repo.Write
            ) 
        
    printfn "Rendered to S3"

    0
