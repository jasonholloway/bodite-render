namespace BoditeRender

open System


type WrappedKey (v: obj) =
    member val Value = v
    member val Hash = v.GetHashCode()

    override x.Equals(o) =
        x.Value.Equals(o)

    override x.GetHashCode() =
        x.Hash

    interface IComparable with
        member x.CompareTo(o) =
            let other = (o :?> WrappedKey)
            x.Hash - other.Hash


            

[<AbstractClass>]
type Page (locale: Locale, keys: obj seq) =

    member val Keys = 
                [[locale :> obj] |> Seq.ofList; keys] 
                |> Seq.concat
                |> Seq.map (fun k -> WrappedKey(k)) 
                |> Set.ofSeq

    abstract member Path : string
    
    member x.Locale = locale




module PageReg =
    let build (pages: seq<Page>) =
        pages
        |> Seq.map (fun p -> (p.Keys, p))
        |> Map.ofSeq
        
    let findPage (registry : Map<Set<WrappedKey>,Page>) (keys : seq<obj>) =
        let wrappedKeys = keys |> Seq.map( fun k -> WrappedKey(k)) |> Set.ofSeq
        registry.TryFind wrappedKeys
        