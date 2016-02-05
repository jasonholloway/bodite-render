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




type LocaleString = {
    LV: string;
    RU: string;
    EN: string;
}

type CategoryNode = {
    Id : string;
    Name : LocaleString;
    Description : LocaleString;
    Children : seq<CategoryNode>;
}


//need to clean up category tree

//would work lots better if categories were stored flat in db, just linking to each other
//...


let getCategories =
    let rec crawl<'N> (c:'N) =        
        seq {
            yield c
            
            if c.Children != null then 
                yield! c.Children 
                        |> Seq.collect (fun n -> crawl n)
        }

    getCategoryTree |> crawl








let getAllProducts = 
    ProductView.Load(allProductsUrl).Rows        
    |> Seq.map (fun r -> r.Value)


