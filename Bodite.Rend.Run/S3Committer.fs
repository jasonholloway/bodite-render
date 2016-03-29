module S3Committer

open System.IO
open BoditeRender
open Amazon.S3



//committer should be a disposable type, subclassable also



let client = new AmazonS3Client()


let concretizePath virtPath =
    match Path.HasExtension(virtPath) with
    | true  -> virtPath
    | false -> (Path.Combine(virtPath, "index.html")).Replace('\\', '/')
               



let create baseDirPath =
    fun (vf: VirtFile) ->
        let blitter = Blitter.create (Array.zeroCreate<byte>(4096))

        let path = Path.Combine (baseDirPath, (vf.Path |> concretizePath))
        let dirPath = Path.GetDirectoryName(path)

        if not <| Directory.Exists(dirPath) then
           Directory.CreateDirectory(dirPath) |> ignore 

        use sFile = File.Create(path)
            
        vf.Data
        |> blitter sFile
