namespace BoditeRender

open FSharp.Data
open System.Text.RegularExpressions
    


module Hydrator =

    ///////////////////////////////////////////////////
    /// PRODUCTS /////////////////////////////////////
             
    [<Literal>]
    let allProductsUrl = "https://jasonholloway.cloudant.com/bb/_design/bb/_view/all-products   "

    type ProductDbView = JsonProvider<allProductsUrl>
    
    let private hydrateProducts () =
        ProductDbView.Load(allProductsUrl).Rows        
        |> Seq.map (fun r -> 
                        {
                            Key = Regex.Match(r.Value.Id, "^product[\/-](.+)").Value
                            Name = {
                                    LV = Some r.Value.Name.Lv
                                    RU = r.Value.Name.Ru
                                    }
                            Description = {
                                            LV = None
                                            RU = None
                                            }
                            MachineName = r.Value.MachineName
                            CategoryKeys = r.Value.CategoryKeys |> Seq.map (fun g -> g.ToString()) |> Seq.toList
                        }
                    )
        |> Seq.map (fun p -> (p.Key, p))
        |> Map.ofSeq




    //////////////////////////////////////////////////
    /// CATEGORIES //////////////////////////////////
        
    [<Literal>]
    let allCategoriesUrl = "https://jasonholloway.cloudant.com/bb/_design/bb/_view/all-categories "

    type private CategoryDbView = JsonProvider<allCategoriesUrl>
    

    type private CatRecord = {
        Key : string
        Name : LocaleString
        Description : LocaleString
        ChildKeys : List<string>   
    }
    

    let private DbRow2CatRecord (r: CategoryDbView.Row) = 
        { 
            Key = r.Key.JsonValue.ToString();  
            Name = { LV = Some r.Value.Name.Value.Lv; RU = None };
            Description = { LV = None; RU = None };
            ChildKeys = r.Value.Children |> Array.map (fun g -> g.ToString()) |> Array.toList
        }
    

    let private buildCategories (catRecs: seq<CatRecord>) (prodMap: Map<string, Product>) =
        let catRecMap = catRecs |> Seq.map (fun r -> (r.Key, r)) |> Map.ofSeq

        let groupedProdMap = prodMap
                                |> Seq.collect (fun kp -> kp.Value.CategoryKeys |> Seq.map (fun catKey -> (catKey, kp.Value)))
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
    

    let private hydrateCategories prods =
        let catRecs =   CategoryDbView.Load(allCategoriesUrl).Rows
                        |> Seq.map DbRow2CatRecord

        buildCategories catRecs prods


        



    let hydrateModel () =
        let prods = hydrateProducts()
        let cats = hydrateCategories(prods)

        Model(prods, cats)



