namespace BoditeRender

open System


type PageKey (v: obj) =
    member val Value = v
    member val Hash = v.GetHashCode()

    override x.Equals(o) =
        x.Value.Equals(o)

    override x.GetHashCode() =
        x.Hash

    interface IComparable with
        member x.CompareTo(o) =
            let other = (o :?> PageKey)
            x.Hash - other.Hash


            

[<AbstractClass>]
type Page (keys: Set<PageKey>) =

    new(keys : obj seq) =
        Page(keys 
             |> Seq.map (fun k -> PageKey(k))
             |> Set.ofSeq)

    new(key : obj) =
        Page([key])
        
    member val Keys = keys

    abstract member Path : string
    abstract member Title : string
