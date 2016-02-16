module FSCommitter

open System.IO
open BoditeRender

let create baseDirPath =
    fun (vf: VirtFile) ->
        let blitter = Shared.getBlitter (Array.zeroCreate<byte>(4096))

        let path = Path.Combine (baseDirPath, vf.Path)
        let dirPath = Path.GetDirectoryName(path)

        if not <| Directory.Exists(dirPath) then
           Directory.CreateDirectory(dirPath) |> ignore 

        use sFile = File.Create(path)
            
        vf.Data
        |> blitter sFile
