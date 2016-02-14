﻿namespace BoditeRender

open FSharp.Data
open System.Text.RegularExpressions
    


module Hydrator =

#if RELEASE
    [<Literal>]
    let dbUrl = "https://jasonholloway.cloudant.com/bb"
#else
    [<Literal>]
    let dbUrl = "http://localhost:5984/bb"
#endif






    //////////////////////////////////////////////////
    // PRODUCTS /////////////////////////////////////
             
    [<Literal>]
    let allProductsUrl = dbUrl + "/_design/bb/_view/all-products"

    type ProductDbView = JsonProvider<allProductsUrl>
    
    let internal hydrateProducts (json: string) =
    
        let buildLocaleString (o: ProductDbView.Name) =        
            LocaleString ((Locales.LV, o.Lv) :: (if o.Ru.IsSome then [(Locales.RU, o.Ru.Value)] else []))
            

        ProductDbView.Parse(json).Rows        
        |> Seq.map (fun r -> 
                        {
                            Key = Regex.Match(r.Value.Id, "^product[\/-](.+)").Value
                            Name = buildLocaleString r.Value.Name
                            Description = LocaleString []
                            CategoryKeys = r.Value.CategoryKeys |> Seq.map (fun g -> g.ToString()) |> Seq.toList
                        }
                    )
        |> Seq.map (fun p -> (p.Key, p))
        |> Map.ofSeq





    /////////////////////////////////////////////////
    // CATEGORIES //////////////////////////////////
        
    [<Literal>]
    let allCategoriesUrl = dbUrl + "/_design/bb/_view/all-categories"

    type private CategoryDbView = JsonProvider<allCategoriesUrl>
    

    type internal CatRec = {
        Key : string
        Name : LocaleString
        Description : LocaleString
        ChildKeys : List<string>   
    }
    

    let private DbRow2CatRecord (r: CategoryDbView.Row) = 
        { 
            Key = if r.Key.Guid.IsSome then r.Key.Guid.Value.ToString() else r.Key.String.Value
            Name = LocaleString ((Locales.LV, r.Value.Name.Value.Lv) :: []) //if r.Value.Name.IsSome then [(Locale.RU, r.Value.Name.Value)] else [] );
            Description = LocaleString [];
            ChildKeys = r.Value.Children |> Array.map (fun g -> g.ToString()) |> Array.toList
        }
    

    let private buildCategories (catRecs: seq<CatRec>) (prodMap: Map<string, Product>) =
        let catRecMap = catRecs |> Seq.map (fun r -> (r.Key, r)) |> Map.ofSeq

        let groupedProdMap = prodMap
                                |> Seq.collect (fun kp -> kp.Value.CategoryKeys |> Seq.map (fun catKey -> (catKey, kp.Value)))
                                |> Seq.groupBy (fun kp -> match kp with | (k, p) -> k)
                                |> Seq.map (fun kr -> 
                                                    let k, r = kr;
                                                    (k, r |> Seq.map (fun kp -> match kp with | (_, p) -> p))
                                                    ) 
                                |> Map.ofSeq


        let rec rec2Cat (map: Map<string, Category>) (r: CatRec) =        
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
                        Products = match groupedProdMap.TryFind(r.Key) with
                                    | Some s -> s |> Seq.toList
                                    | None -> []
                    }                

                    let map = map |> Map.add cat.Key cat                
                
                    (map, cat)
                
        catRecs 
        |> Seq.fold 
                (fun m r -> match (rec2Cat m r) with | (m, _) -> m) 
                Map.empty<string, Category>
    

    let internal hydrateCategories (json: string) prods =
        let catRecs =   CategoryDbView.Parse(json).Rows
                        |> Seq.map DbRow2CatRecord

        buildCategories catRecs prods


        



    let hydrateModel () =
        let prods = hydrateProducts allProductsUrl
        let cats = hydrateCategories allCategoriesUrl prods

        Model(prods, cats)



