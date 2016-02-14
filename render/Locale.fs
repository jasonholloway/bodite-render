namespace BoditeRender

open System

type Locale (code: string) =
    member x.Code = code

    interface IComparable with
        member x.CompareTo (o: obj) =
            x.Code.CompareTo( (o :?> Locale).Code )



module Locales =
    let LV = Locale "LV"
    let RU = Locale "RU"
   


type LocaleString (entries: (Locale * string) list) = 
    let map = entries |> Map.ofList

    member x.get locale =
        match map.TryFind(locale) with
        | Some s -> Some s
        | None -> None //should try to get default first...
        