namespace BoditeRender

module CouchDbLoader =

    open Newtonsoft.Json
    open FSharp.Data
    open System


    type CouchProduct = {
        Id: string
        Name: Map<string, string>
        Description: Map<string, string>
        CategoryKeys: string list
    }
    
    type CouchCategory = {
        Id: string
        Name: Map<string, string>
        Children: string list
    }


    type CouchViewRow<'V> = {
        Key: string
        Value: 'V
    }

    type CouchView<'V> = {
        Rows: CouchViewRow<'V> list
    }

    

    let loadProducts (json : string) =
        let v = JsonConvert.DeserializeObject<CouchView<CouchProduct>>(json)

        v.Rows
        |> List.map (fun r ->
                            {
                                DbProduct.Key = r.Key
                                Name = r.Value.Name
                                Description = r.Value.Description
                                CategoryKeys = r.Value.CategoryKeys
                            }
                        )

        

    let loadCategories (json: string) =
        let v = JsonConvert.DeserializeObject<CouchView<CouchCategory>>(json)

        v.Rows
        |> List.map (fun r ->
                            {
                                DbCategory.Key = r.Key
                                Name = r.Value.Name
                                ChildKeys = r.Value.Children
                            }
                        )

                        