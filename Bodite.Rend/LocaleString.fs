namespace BoditeRender


type LocaleString (m: Map<Locale, string>) =

    new(entries: (Locale * string) list) = 
        LocaleString(entries |> Map.ofList)

    member x.get locale =
        match m.TryFind(locale) with
        | Some s -> Some s
        | None -> if locale.Equals(Locales.Default) then None else x.get(Locales.Default)  //should try to get default first...
        