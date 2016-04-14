namespace BoditeRender

open System
open System.IO



type VirtFile (path, data: byte[]) =
    new (p, s : string) =
        let d = Text.ASCIIEncoding.UTF8.GetBytes(s)
        new VirtFile(p, d)

    member val Path: string = path
    member val Data: byte[] = data

    interface IDisposable with
        member x.Dispose () =
//            x.Data.Dispose()
            ()



[<AbstractClass>]
type FileCommitter () as o =

    abstract member Commit : VirtFile -> unit
    abstract member Dispose : unit -> unit
    
    interface IDisposable with
        member x.Dispose() = o.Dispose()


     