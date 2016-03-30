namespace BoditeRender

open System


[<AbstractClass>]
type Committer () as o =

    abstract member Commit : VirtFile -> unit
    abstract member Dispose : unit -> unit
    
    interface IDisposable with
        member x.Dispose() = o.Dispose()


     