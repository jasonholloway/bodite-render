namespace BoditeRender

open System.IO
open Amazon.S3
open Amazon.S3.Model




type S3Repo (client : Amazon.S3.AmazonS3Client, bucketName : string) =
    inherit FileRepo ()
    

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


    override x.Read (path : string) =
        failwith "notimpl"


    override x.Write (vf : VirtFile) =

        let req = new PutObjectRequest()
        
        req.BucketName <- bucketName
        req.ContentType <- "text/html"
        req.Key <- if vf.Path.Equals("") then "index.html" else vf.Path
        
        use str = new MemoryStream(vf.Data)

        req.InputStream <- str

        let resp = client.PutObject(req)

        ()


    override x.Remove (path : string) =
        failwith "notimpl"

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
