namespace BoditeRender


type LocaleString (m: Map<Locale, string>) =

    new(entries: (Locale * string) list) = 
        LocaleString(entries |> Map.ofList)

    member val Map = m

    member x.get locale =
        match m.TryFind(locale) with
        | Some s -> Some s
        | None -> if locale.Equals(Locales.Default) then None else x.get(Locales.Default)  //should try to get default first...
        
    override x.Equals other =
        match other with
        | :? LocaleString as o -> x.Map = o.Map 
        | _ -> false

    override x.GetHashCode () =
        x.Map.GetHashCode()

    interface System.IComparable with
        member x.CompareTo (other) =
            compare (x.GetHashCode()) (other.GetHashCode())

             
