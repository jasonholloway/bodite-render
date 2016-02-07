module Pages

open Products
open Categories


type Page (path, title) =
    member x.Path : string = "Brigitas Bodite - " + path
    member x.Title : string = title
        

type ProductPage (prod, cat) = 
    inherit Page(
                "produkti/" + prod.MachineName, 
                if prod.Name.LV.IsSome then prod.Name.LV.Value else "TITLE")

    member val Product : Product = prod
    member val Category: Category = cat
        

type CategoryPage (cat: Category) =
    inherit Page(
                "categories/" + (defaultArg cat.Name.LV "blah"), 
                defaultArg cat.Name.LV "blah")

    member val Category = cat




type Model (?products, ?categories) = 
    member val Products = (defaultArg products Seq.empty<Product>) |> Seq.toList
    member val Categories = (defaultArg categories Seq.empty<Category>) |> Seq.toList




let buildPages (m: Model) =    
    seq {
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