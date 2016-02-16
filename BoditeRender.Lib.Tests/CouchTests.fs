module CouchDbLoader

open System
open NUnit.Framework
open FsUnit
open FSharp.Data

open BoditeRender
open CouchDbLoader
open Json


//NO TESTING OF STRUCTURE - THIS IS JUST DESERIALISATION + NORMALISATION


let rand = Random()

let getRandomJsonLocaleString () =
    JObj []

let createJsonProductRow (key) =
    let id = "product-" + key
 
    JObj [
            JProp("id", !!id)
            JProp("key", !!key)
            JProp("value", JObj [            
                                JProp("_id", !!id)
                                JProp("_rev", !!Guid.NewGuid().ToString())
                                JProp("name", getRandomJsonLocaleString())
                                JProp("description", getRandomJsonLocaleString())
                                JProp("categoryKeys", JArr [ for i in [0..3] -> !! Guid.NewGuid().ToString() ])
                                JProp("images", JArr [])
                                ])
            ]


let createJsonCategoryRow (key: string) (childKeys: string list) =
    let id = "category-" + key

    JObj [
            JProp("id", !!id)
            JProp("key", !!key)
            JProp("value", JObj [
                                JProp("_id", !!id)
                                JProp("_rev", !!Guid.NewGuid().ToString())
                                JProp("name", getRandomJsonLocaleString())
                                JProp("children", JArr [ for ck in childKeys -> !!ck ])
                                ])
         ]





[<TestFixture>]
type ``loadProducts`` () =
    let keys = [ for i in [1..50] -> Guid.NewGuid().ToString() ] 
    
    let exampleJson =  
        (JObj [
                JProp("rows", JArr [
                                    for k in keys -> createJsonProductRow(k)
                                    ])
             ]
        |> toJson).ToString()
    
    
    [<Test>]
    member x.``takes json as argument`` () = 
        CouchDbLoader.loadProducts exampleJson
        |> ignore

    [<Test>]
    [<ExpectedException>]
    member x.``dislikes non-json`` () =
        CouchDbLoader.loadProducts "oi21h3oi21h3knlkn" |> ignore
        

    [<Test>]
    member x.``returns DbProduct list`` () =
        CouchDbLoader.loadProducts exampleJson
        |> should be ofExactType<DbProduct list>

        
    [<Test>]
    member x.``returns DbProduct list with expected keys`` () =
        let r = CouchDbLoader.loadProducts exampleJson

        r.Length 
        |> should equal keys.Length

        r 
        |> Seq.map (fun p -> p.Key)
        |> should equal keys






type catSpec = {
    Key: string
    ChildKeys: string list
}


[<TestFixture>]
type ``loadCategories`` () =
    let specs = [ 
                    for i in [1..50] -> { 
                                        Key = Guid.NewGuid().ToString()
                                        ChildKeys = [ for j in [1..3] -> Guid.NewGuid().ToString() ]
                                      } 
                ] 
    
    let exampleJson =  
        (JObj [
                JProp("rows", JArr [ for s in specs -> (createJsonCategoryRow s.Key s.ChildKeys) ])
             ]
        |> toJson).ToString()
    
    
    [<Test>]
    member x.``takes json as argument`` () = 
        CouchDbLoader.loadCategories exampleJson
        |> ignore

    [<Test>]
    [<ExpectedException>]
    member z.``dislikes non-json`` () =
        CouchDbLoader.loadCategories "oi21h3oi21h3knlkn" |> ignore
        

    [<Test>]
    member x.``returns DbCategory list`` () =
        CouchDbLoader.loadCategories exampleJson
        |> should be ofExactType<DbCategory list>

        
    [<Test>]
    member x.``returns DbCategory list with expected keys and childkeys`` () =
        let r = CouchDbLoader.loadCategories exampleJson

        r.Length 
        |> should equal specs.Length

        r 
        |> Seq.map (fun p -> { Key=p.Key; ChildKeys=p.ChildKeys })
        |> should equal specs


    [<Test>]
    member x.``null ChildKeys defaults to empty list`` () =
        let dbCats = CouchDbLoader.loadCategories @"{
                                                        ""rows"": [  
                                                            {
                                                                ""key"": ""8768767"",
                                                                ""value"": {
                                                                    ""_id"": ""lkjlkjlkjl"",
                                                                    ""_rev"": ""1287398273982173"",
                                                                    ""name"": {
                                                                        ""LV"": ""lkjlkjlkj""
                                                                    }
                                                                }
                                                            }
                                                        ] 
                                                    }"
        dbCats.Length |> should equal 1
        dbCats.Head.ChildKeys |> should equal []


