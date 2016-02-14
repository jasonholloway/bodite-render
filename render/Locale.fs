namespace BoditeRender


type Locale =
    | LV = 0
    | RU = 1


type LocaleString (entries: (Locale * string) list) = 
    let map = entries |> Map.ofList

    member x.get locale =
        match map.TryFind(locale) with
        | Some s -> Some s
        | None -> None //should try to get default first...
        