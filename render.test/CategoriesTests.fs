module CategoriesTests

open System
open NUnit.Framework
open FsUnit
open FSharp.Data
open Categories


let rec createCat isRoot depth =
    {
        Key = if isRoot then "root" else System.Guid.NewGuid().ToString()
        Name = { LV=""; RU="" }
        Description = { LV=""; RU="" }
        Children = match depth with
                    | 0 -> []
                    | _ -> [0..3] |> List.map (fun i -> createCat false (depth - 1))
    }
    

let rec flattenCats cat =
    seq {
        yield cat
        yield! cat.Children |> Seq.collect (fun c -> flattenCats c)
    }

    

let rootCat = createCat true 4

let catRecs = flattenCats rootCat
                |> Seq.map (fun cat -> { 
                                        Key = cat.Key
                                        Name = cat.Name
                                        Description = cat.Description
                                        ChildKeys = cat.Children |> List.map (fun c -> c.Key)
                                    })
                                  

[<Test>]
let ``Ensure correct number of catRecords have been created for testing`` () =
    catRecs |> Seq.toList |> List.length 
    |> should equal (1 + 4 + 16 + 64 + 256)


[<Test>]
let ``Categories.buildCategories includes all cats in map`` () =
    let builtKeys = buildCategories catRecs 
                    |> Map.toSeq
                    |> Seq.map (fun kv -> match kv with | (k, v) -> k)
                    |> Set.ofSeq

    let catKeys = catRecs |> Seq.map (fun r -> r.Key) |> Set.ofSeq
    
    builtKeys |> should equal catKeys


[<Test>]
let ``Categories.buildCategories articulates cat children`` () =
    let builtMap = buildCategories catRecs

    catRecs 
    |> Seq.forall 
            (fun r -> 
                builtMap.[r.Key].Children 
                |> List.map (fun c -> c.Key) 
                |> should equal r.ChildKeys
                true
            )
    |> should equal true
    


