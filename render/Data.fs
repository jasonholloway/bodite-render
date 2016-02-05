module Data

open FSharp.Data

[<Literal>]
let allProductsUrl = "https://jasonholloway.cloudant.com/bb/_design/bb/_view/all_products"

type ProductView = JsonProvider<allProductsUrl>



let getAllProducts = 
    ProductView.Load(allProductsUrl).Rows        
    |> Seq.map (fun r -> r.Value)


