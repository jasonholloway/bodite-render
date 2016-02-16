namespace BoditeRender

open FSharp.Data
open Flurl

module CouchDbLoader =

    open Newtonsoft.Json
    open FifteenBelow.Json
    open FSharp.Data
    open System
    open System.Collections.Generic


    type CouchProduct = {
        Id: string
        Name: Map<string, string>
        Description: Map<string, string>
        CategoryKeys: string list
    }
    
    type CouchCategory = {
        Id: string
        Name: Map<string, string>
        Children: string list option
    }


    type CouchViewRow<'V> = {
        Key: string
        Value: 'V
    }

    type CouchView<'V> = {
        Rows: CouchViewRow<'V> list
    }



    let jsonConverters = [|
                            OptionConverter() :> JsonConverter
                         |]

    let defaultNull subject def =
        if obj.Equals(subject, null) then def else subject


    let loadProducts (json : string) =
        let v = JsonConvert.DeserializeObject<CouchView<CouchProduct>>(json, converters=jsonConverters)

        v.Rows
        |> List.map (fun r ->
                            {
                                DbProduct.Key = r.Key
                                Name = defaultNull r.Value.Name Map.empty
                                Description = defaultNull r.Value.Description Map.empty
                                CategoryKeys = r.Value.CategoryKeys
                            }
                        )

        

    let loadCategories (json: string) =
        let v = JsonConvert.DeserializeObject<CouchView<CouchCategory>>(json, converters=jsonConverters)

        v.Rows
        |> List.map (fun r ->
                            {
                                DbCategory.Key = r.Key
                                Name = defaultNull r.Value.Name Map.empty
                                ChildKeys = defaultArg r.Value.Children []
                            }
                        )

                        



    let loadDbModel (baseUrl: string) =
        let allProductsRelUrl = "_design/bb/_view/all-products"
        let allCategoriesRelUrl = "_design/bb/_view/all-categories"
        
        let categories = Http.RequestString(
                                    Url.Combine(baseUrl, allCategoriesRelUrl),
                                    httpMethod="GET",
                                    headers=[("Accept", "application/json")]
                                    )
                            |> loadCategories


        let products = Http.RequestString(
                                    Url.Combine(baseUrl, allProductsRelUrl),
                                    httpMethod="GET",
                                    headers=[("Accept", "application/json")]
                                    )
                            |> loadProducts

        DbModel(categories, products)