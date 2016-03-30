namespace BoditeRender

open System.IO
open Amazon.S3





type S3Committer () =
    inherit FileCommitter ()
    
    let client = new AmazonS3Client()
    

    override x.Commit (vf : VirtFile) =
        ()


    override x.Dispose () =
        client.Dispose()
        

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
