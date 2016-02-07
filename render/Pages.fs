module Pages

open Products
open Categories


//
//type IPage =
//    abstract member Path : string
//    abstract member Title : string


type Page (path, title) =
    member x.Path : string = "Brigitas Bodite - " + path
    member x.Title : string = title



//type CategoryPage =
//    interface IPage with
//        member x.Path = ""
//        member x.Title = ""





type ProductPage (prod, cat) = 
    inherit Page(
                "produkti/" + prod.MachineName, 
                if prod.Name.LV.IsSome then prod.Name.LV.Value else "TITLE")

    member x.Product : Product = prod
    member x.Category: Category = cat



//
//type CategoryPage (cat: Category) =
//    inherit Page(
//                "categories/" + cat.Name.LV, 
//                cat.name.lv)




type Model (?products, ?categories) = 
    member val Products = (defaultArg products Seq.empty<Product>) |> Seq.toList
    member val Categories = (defaultArg categories Seq.empty<Category>) |> Seq.toList




let buildPages (m: Model) =    
    List.empty<Page>



//buildPages should really be testable... which requires 







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