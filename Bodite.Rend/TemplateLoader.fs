namespace BoditeRender

open System


[<AbstractClass>]
type TemplateLoader () as o =

    abstract member Load : string -> string
    abstract member Dispose : unit -> unit
    
    interface IDisposable with
        member x.Dispose() = o.Dispose()


     