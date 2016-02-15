module FSCommitter

open System.IO
open BoditeRender

let create dirPath =
    fun (vf: VirtFile) ->
        let blitter = Shared.getBlitter (Array.zeroCreate<byte>(4096))

        use sFile = File.Create (Path.Combine(dirPath, vf.Path))
            
        vf.Data
        |> blitter sFile
