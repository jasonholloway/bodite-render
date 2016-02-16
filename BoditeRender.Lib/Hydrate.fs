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




        


    let hydrateModel (dbModel: DbModel) =
        let prods = dbModel.Products
                    |> hydrateProducts 
                    |> Seq.map (fun p -> (p.Key, p))
                    |> Map.ofSeq
                    
        let cats = dbModel.Categories
                   |> hydrateCategories prods 
                   |> Seq.map (fun c -> (c.Key, c))
                   |> Map.ofSeq

        Model(prods, cats)


