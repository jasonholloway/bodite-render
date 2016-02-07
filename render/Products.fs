module Products

open Shared
open Categories
open FSharp.Data

[<Literal>]
let allProductsUrl = "https://jasonholloway.cloudant.com/bb/_design/bb/_view/all_products "


type ProductDbView = JsonProvider<allProductsUrl>


type Product = {
    Id: string;
    Name: LocaleString;
    Description: LocaleString;
    Categories: Category[];
}



let getAllProducts = 
    ProductDbView.Load(allProductsUrl).Rows        
    |> Seq.map (fun r -> r.Value)


