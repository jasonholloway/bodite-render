module Categories

open Shared
open Products
open FSharp.Data


[<Literal>]
let allCategoriesUrl = "https://jasonholloway.cloudant.com/bb/_design/bb/_view/all-categories "

type CategoryDbView = JsonProvider<allCategoriesUrl>



type CatRecord = {
    Key : string
    Name : LocaleString
    Description : LocaleString
    ChildKeys : List<string>   
}


type Category = {
    Key : string
    Name : LocaleString
    Description : LocaleString
    Children : List<Category> 
    Products : List<Product>   
}



let DbRow2CatRecord (r: CategoryDbView.Row) = 
    { 
        Key = r.Key.JsonValue.ToString();  
        Name = { LV = Some r.Value.Name.Value.Lv; RU = None };
        Description = { LV = None; RU = None };
        ChildKeys = r.Value.Children |> Array.map (fun g -> g.ToString()) |> Array.toList
    }
    

let buildCategories (catRecs: seq<CatRecord>) products =
    let catRecMap = catRecs |> Seq.map (fun r -> (r.Key, r)) |> Map.ofSeq

    let prodMap = products
                    |> Seq.collect (fun p -> p.CategoryKeys |> Seq.map (fun k -> (k, p)))
                    |> Seq.groupBy (fun kp -> match kp with | (k, p) -> k)
                    |> Seq.map (fun kr -> 
                                        let k, r = kr;
                                        (k, r |> Seq.map (fun kp -> match kp with | (_, p) -> p))
                                        ) 
                    |> Map.ofSeq


    let rec rec2Cat (map: Map<string, Category>) (r: CatRecord) =        
        match map.TryFind(r.Key) with
        | Some cat -> 
                (map, cat)
        | None ->
                let map, children = r.ChildKeys 
                                        |> List.fold
                                            (fun mr k -> 
                                                let map, siblings = mr
                                                let map, child = rec2Cat map catRecMap.[k]                                    
                                                (map, child :: siblings) 
                                              )
                                            (map, [])
                let cat = {
                    Key = r.Key
                    Name = r.Name
                    Description = r.Description
                    Children = children |> List.rev
                    Products = match prodMap.TryFind(r.Key) with
                                | Some s -> s |> Seq.toList
                                | None -> []
                }                

                let map = map |> Map.add cat.Key cat                
                
                (map, cat)
                
    catRecs 
    |> Seq.fold 
            (fun m r -> match (rec2Cat m r) with | (m, _) -> m) 
            Map.empty<string, Category>
    
    


let getCategoryMap () =
    CategoryDbView.Load(allCategoriesUrl).Rows
    |> Seq.map DbRow2CatRecord
    |> buildCategories






