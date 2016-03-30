namespace BoditeRender

open System
open System.IO



type VirtFile (path, data: Stream) =
    new (path, data: string) =
        let str = new MemoryStream(Text.ASCIIEncoding.UTF8.GetBytes(data))
        new VirtFile(path, str)

    member val Path: string = path
    member val Data: Stream = data

    interface IDisposable with
        member x.Dispose () =
            x.Data.Dispose()



[<AbstractClass>]
type FileCommitter () as o =

    abstract member Commit : VirtFile -> unit
    abstract member Dispose : unit -> unit
    
    interface IDisposable with
        member x.Dispose() = o.Dispose()


     