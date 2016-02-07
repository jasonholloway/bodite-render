namespace BoditeRender


type Page (path, title) =
    member x.Path : string = path

    member x.Title : string = "Brigitas Bodite" + match title with
                                                    | Some t -> " - " + t
                                                    | None -> ""

                                                    
type HomePage () =
    inherit Page(
                path = "",
                title = None)

    member val FeaturedProducts : List<Product> = List.empty
    


type ProductPage (prod: Product, cat: Category) = 
    inherit Page(
                path = Paths.resolvePath prod, 
                title = prod.Name.LV)

    member val Product : Product = prod
    member val Category: Category = cat
        


type CategoryPage (cat: Category) =
    inherit Page(
                path = Paths.resolvePath cat, 
                title = cat.Name.LV)

    member val Category = cat
    





module Pages =
   
    let buildPages (m: Model) =    
        seq {
            yield new HomePage() :> Page

            yield! m.Categories
                    |> Seq.collect (fun c -> seq { 
                                                yield new CategoryPage(c) :> Page

                                                yield! c.Products 
                                                       |> Seq.map (fun p -> new ProductPage(p, c) :> Page) 

                                                })

            //...
        }
        |> Seq.toList







    //should be one page per product*category combination
    //plus category field should be filled in here...

    //
    //let getProductPages =
    //    Data.getAllProducts
    //    |> Seq.filter (fun p -> p.MachineName.IsSome)
    //    |> Seq.map (fun p -> {
    //                            path = if p.MachineName.IsSome then ("produkti/" + p.MachineName.Value) else "";
    //                            title = "Brigitas Bodite - Produkti - " + p.Name.Lv;  
    //                            product = p;
    //                            category = 13;
    //                        })