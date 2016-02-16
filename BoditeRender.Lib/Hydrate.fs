namespace BoditeRender

open FSharp.Data
open System.Text.RegularExpressions


module Hydrate =
        
    let hydrateLocaleString (m: Map<string, string>) = 
        m
        |> Map.toSeq
        |> Seq.map (fun (k, v) -> ((Locales.getByKey k), v))
        |> Seq.filter (fun (l, v) -> l.IsSome)
        |> Seq.map (fun (l, v) -> (l.Value, v))
        |> Map.ofSeq
        |> (fun newMap -> new LocaleString(newMap))
    



    let hydrateProducts (dbProds: DbProduct list) =    
        dbProds
        |> List.map (fun p -> {
                                Product.Key = p.Key
                                Name = hydrateLocaleString p.Name
                                Description = hydrateLocaleString p.Description
                                CategoryKeys = p.CategoryKeys
                              })

                              
                                  
    let hydrateCategories (prodMap: Map<string, Product>) (dbCats: DbCategory list) =
    
        let dbCatMap = dbCats |> Seq.map (fun c -> (c.Key, c)) |> Map.ofSeq

        let prodsByCatMap = prodMap
                                |> Map.toSeq                                
                                |> Seq.collect (fun (_, p) -> p.CategoryKeys |> Seq.map (fun catKey -> (catKey, p)))
                                |> Seq.groupBy (fun (catKey, _) -> catKey)
                                |> Seq.map (fun (k, r) -> (k, r |> Seq.map (fun (_, p) -> p)) ) 
                                |> Map.ofSeq
                              
                                               
        let rec rec2Cat (map: Map<string, Category>) (dbCat: DbCategory) =        
            match map.TryFind(dbCat.Key) with
            | Some cat -> 
                    (map, cat)
            | None ->
                    let map, children = dbCat.ChildKeys 
                                        |> List.fold (fun mr k -> 
                                                        let map, siblings = mr
                                                        let map, child = rec2Cat map dbCatMap.[k]                                    
                                                        (map, child :: siblings) 
                                                     )
                                                     (map, [])
                    let cat = {
                        Category.Key = dbCat.Key
                        Name = hydrateLocaleString dbCat.Name
                        Description = LocaleString [] // hydrateLocaleString dbCat.Description
                        Children = children |> List.rev
                        Products = match prodsByCatMap.TryFind(dbCat.Key) with
                                    | Some s -> s |> Seq.toList
                                    | None -> []
                    }                

                    let map = map |> Map.add cat.Key cat                
                
                    (map, cat)
                
        dbCats 
        |> Seq.fold 
                (fun m r -> match (rec2Cat m r) with | (m, _) -> m) 
                Map.empty
        |> Map.toSeq
        |> Seq.map (fun (_, c) -> c)
        |> Seq.toList






//
//
//    /////////////////////////////////////////////////
//    // CATEGORIES //////////////////////////////////
//        
//    [<Literal>]
//    let allCategoriesUrl = dbUrl + "/_design/bb/_view/all-categories"
//
//    type private CategoryDbView = JsonProvider<allCategoriesUrl>
//    
//
//    type internal CatRec = {
//        Key : string
//        Name : LocaleString
//        Description : LocaleString
//        ChildKeys : List<string>   
//    }
//    
//
//    let private DbRow2CatRecord (r: CategoryDbView.Row) = 
//        { 
//            Key = if r.Key.Guid.IsSome then r.Key.Guid.Value.ToString() else r.Key.String.Value
//            Name = LocaleString ((Locales.LV, r.Value.Name.Value.Lv) :: []) //if r.Value.Name.IsSome then [(Locale.RU, r.Value.Name.Value)] else [] );
//            Description = LocaleString [];
//            ChildKeys = r.Value.Children |> Array.map (fun g -> g.ToString()) |> Array.toList
//        }
//    
//
//    let private buildCategories (catRecs: seq<CatRec>) (prodMap: Map<string, Product>) =
//        let catRecMap = catRecs |> Seq.map (fun r -> (r.Key, r)) |> Map.ofSeq
//
//        let groupedProdMap = prodMap
//                                |> Seq.collect (fun kp -> kp.Value.CategoryKeys |> Seq.map (fun catKey -> (catKey, kp.Value)))
//                                |> Seq.groupBy (fun kp -> match kp with | (k, p) -> k)
//                                |> Seq.map (fun kr -> 
//                                                    let k, r = kr;
//                                                    (k, r |> Seq.map (fun kp -> match kp with | (_, p) -> p))
//                                                    ) 
//                                |> Map.ofSeq
//
//
//        let rec rec2Cat (map: Map<string, Category>) (r: CatRec) =        
//            match map.TryFind(r.Key) with
//            | Some cat -> 
//                    (map, cat)
//            | None ->
//                    let map, children = r.ChildKeys 
//                                            |> List.fold
//                                                (fun mr k -> 
//                                                    let map, siblings = mr
//                                                    let map, child = rec2Cat map catRecMap.[k]                                    
//                                                    (map, child :: siblings) 
//                                                  )
//                                                (map, [])
//                    let cat = {
//                        Key = r.Key
//                        Name = r.Name
//                        Description = r.Description
//                        Children = children |> List.rev
//                        Products = match groupedProdMap.TryFind(r.Key) with
//                                    | Some s -> s |> Seq.toList
//                                    | None -> []
//                    }                
//
//                    let map = map |> Map.add cat.Key cat                
//                
//                    (map, cat)
//                
//        catRecs 
//        |> Seq.fold 
//                (fun m r -> match (rec2Cat m r) with | (m, _) -> m) 
//                Map.empty<string, Category>
//    
//
//    let internal hydrateCategories (json: string) prods =
//        let catRecs =   CategoryDbView.Parse(json).Rows
//                        |> Seq.map DbRow2CatRecord
//
//        buildCategories catRecs prods
//

        



    let hydrateModel () =
//        let prods = hydrateProds allProductsUrl
//        let cats = hydrateCategories allCategoriesUrl prods
//
//        Model(prods, cats)
        Model()


