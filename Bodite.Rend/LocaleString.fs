namespace BoditeRender

open System.Runtime.Remoting.Messaging

type LocaleString (m: Map<Locale, string>) =

    new(entries: (Locale * string) list) = 
        LocaleString(entries |> Map.ofList)

    member val Map = m

    member x.getString locale =
        match x.Map.TryFind(locale) with
        | Some v -> v
        | None ->
            match x.Map.TryFind(Locales.Default) with
            | Some v -> v
            | None -> ""
           
    override x.ToString() =
        match CallContext.LogicalGetData("bodite-locale") with
        | :? Locale as l -> x.getString(l)
        | _ -> x.getString(Locales.Default)

    override x.Equals other =
        match other with
        | :? LocaleString as o -> x.Map = o.Map 
        | _ -> false

    override x.GetHashCode () =
        x.Map.GetHashCode()

    interface System.IComparable with
        member x.CompareTo (other) =
            compare (x.GetHashCode()) (other.GetHashCode())

             
