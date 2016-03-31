namespace BoditeRender

open System.IO


type FSCommitter (baseDirPath : string) =
    inherit FileCommitter ()
    
    member x.ConcretizePath virtPath =
        match Path.HasExtension(virtPath) with
        | true  -> virtPath
        | false -> (Path.Combine(virtPath, "index.html")).Replace('\\', '/')
               

    override x.Commit (vf: VirtFile) =    
        let blitter = Blitter.create (Array.zeroCreate<byte>(4096))
        
        let path = Path.Combine (baseDirPath, (vf.Path |> x.ConcretizePath))
        let dirPath = Path.GetDirectoryName(path)

        if not <| Directory.Exists(dirPath) then
            Directory.CreateDirectory(dirPath) |> ignore 

        use sFile = File.Create(path)
            
        vf.Data
        |> blitter sFile


    override x.Dispose () =
        ()
