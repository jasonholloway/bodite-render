namespace BoditeRender

open System.IO
open Amazon.S3
open Amazon.S3.Model




type S3Committer () =
    inherit FileCommitter ()
    
    let client =     
        let s3Config = new AmazonS3Config()
        s3Config.ServiceURL <- "http://localhost:9988"
        s3Config.UseHttp <- true
        s3Config.ReadEntireResponse <- true
        s3Config.ForcePathStyle <- true
        
        let creds = Amazon.Runtime.BasicAWSCredentials("", "")
        
        new AmazonS3Client(creds, s3Config)


    //-----------------------------------------------------------------------
    //file of filepath/hashes should be readable here (or maybe above)
    //only files with different hashes should be committed
    //newly orphaned files should be cleaned away
    //-----------------------------------------------------------------------
     

    //-----------------------------------------------------------------------
    //makes me think that design should be such that bare minimum of design is actually
    //on html page, which should mainly be semantic data.
    //hive off to changable css and js files. Thereby few changes should propogate to html.
    //-----------------------------------------------------------------------



    override x.Commit (vf : VirtFile) =

        let req = new PutObjectRequest()
        
        req.BucketName <- "Bodite"
        req.ContentType <- "text/html"
        req.Key <- if vf.Path.Equals("") then "index.html" else vf.Path
        
        req.InputStream <- vf.Data

        //req.ContentBody <- "blah blah blah"

        let resp = client.PutObject(req)

        ()


    override x.Dispose () =
        //client.Dispose()
        ()
        

//
//    let concretizePath virtPath =
//        match Path.HasExtension(virtPath) with
//        | true  -> virtPath
//        | false -> (Path.Combine(virtPath, "index.html")).Replace('\\', '/')
//               
//               
//    let create baseDirPath =
//        fun (vf: VirtFile) ->
//            let blitter = Blitter.create (Array.zeroCreate<byte>(4096))
//
//            let path = Path.Combine (baseDirPath, (vf.Path |> concretizePath))
//            let dirPath = Path.GetDirectoryName(path)
//
//            if not <| Directory.Exists(dirPath) then
//               Directory.CreateDirectory(dirPath) |> ignore 
//
//            use sFile = File.Create(path)
//            
//            vf.Data
//            |> blitter sFile
