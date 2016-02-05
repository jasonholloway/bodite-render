module Data

open FSharp.Data

[<Literal>]
let allProductsUrl = "https://jasonholloway.cloudant.com/bb/_design/bb/_view/all_products "

[<Literal>]
let categoryTreeUrl = "https://jasonholloway.cloudant.com/bb/categorytree"


type ProductView = JsonProvider<allProductsUrl>
type CategoryTree = JsonProvider<categoryTreeUrl>




let getCategoryTree =
    CategoryTree.Load(categoryTreeUrl)



type CategoryNode = {
    _id : string;
    JsonValue : string;
    Children : CategoryNode[];
}


let getCategories =
    let crawl (c:CategoryNode) =        
        seq {
            yield c
            
            if c.Children != null then 
                yield! (c.Children |> crawl) //RECURSIVE!!!!!!
        }
     
    getCategoryTree |> crawl








let getAllProducts = 
    ProductView.Load(allProductsUrl).Rows        
    |> Seq.map (fun r -> r.Value)


