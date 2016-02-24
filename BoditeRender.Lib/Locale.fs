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

    let Default = LV

    let All = [LV; RU]


    let private _m = All 
                     |> List.map (fun l -> (l.Code, l)) 
                     |> Map.ofList 

    let getByKey k =
        _m.TryFind k
        
   


type LocaleString (m: Map<Locale, string>) =

    new(entries: (Locale * string) list) = 
        LocaleString(entries |> Map.ofList)

    member x.get locale =
        match m.TryFind(locale) with
        | Some s -> Some s
        | None -> if locale.Equals(Locales.Default) then None else x.get(Locales.Default)  //should try to get default first...
        