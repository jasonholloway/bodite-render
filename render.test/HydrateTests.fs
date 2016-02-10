module Hydrator

open System
open NUnit.Framework
open FsUnit
open FSharp.Data

open BoditeRender
open Hydrator
open Json

//
//let rec private createCat isRoot depth =
//    {
//        Key = if isRoot then "root" else System.Guid.NewGuid().ToString()
//        Name = { LV = None; RU = None }
//        Description = { LV = None; RU = None }
//        Children = match depth with
//                    | 0 -> []
//                    | _ -> [0..3] |> List.map (fun i -> createCat false (depth - 1))
//        Products = []
//    }



type CatNode = {
    Key: string
    Children: CatNode list
} 


let createCatJson branching depth =
    //first create simple cat tree to ensure parent-child integrity; then flatten tree immediately
    let rec createCatBranch isRoot b d =
        {
            Key = if isRoot then "root" else Guid.NewGuid().ToString()
            Children = match d with
                        | 0 -> []
                        | _ -> [0..b] |> List.map (fun _ -> createCatBranch false b (d - 1))
        }
        
    let rootNode = createCatBranch true branching depth

    let nodes = Helpers.flatten
                            (fun n -> n.Children)
                            rootNode

    let json = 
        JObj [
            JProp("total_rows", !! (nodes |> Seq.length))
            JProp("offset", !! 0)
            JProp(
                "rows", 
                JArr (nodes |> Seq.map (fun n -> 
                                            JObj [
                                                JProp("id", !! "")
                                                JProp("key", !! n.Key)
                                                JProp("value", JObj [                    
                                                                JProp("_id", !! "")
                                                                JProp("_rev", !! "")                            
                                                                JProp("key", !! n.Key)
                                                                JProp("name", JObj [
                                                                                JProp("LV", !! "") 
                                                                                ])                                                                                
                                                                JProp("description", JObj [
                                                                                        JProp("LV", !! "") 
                                                                                        ])
                                                                JProp("children", JArr (n.Children |> Seq.map (fun k -> !! k.Key)))
                                                                ])
                                            ]
                                            )))
        ]
     
    let jsonString = (json |> toJson).ToString()
    
    ( rootNode, jsonString )





let private createProd catKeys =
    {
        Key = System.Guid.NewGuid().ToString()
        Name = { LV = None; RU = None }
        Description = { LV = None; RU = None }
        MachineName = ""
        CategoryKeys = catKeys
    }
    
//
//let rec private flattenCats cat =
//    seq {
//        yield cat
//        yield! cat.Children |> Seq.collect (fun c -> flattenCats c)
//    }

    
//
//let private rootCat = createCat true 4

//
//let private catRecs = flattenCats rootCat
//                        |> Seq.map (fun cat -> {
//                                            Key = cat.Key
//                                            Name = cat.Name
//                                            Description = cat.Description
//                                            ChildKeys = cat.Children |> List.map (fun c -> c.Key)
//                                        })
                                  



//
//[<Test>]
//let ``Ensure correct number of catRecords have been created for testing`` () =
//    catRecs |> Seq.toList |> List.length 
//    |> should equal (1 + 4 + 16 + 64 + 256)



[<TestFixture>]
type ``hydrateCategories`` () =
    [<Test>]
    member x.``includes all cats in map`` () =
        let root, json = createCatJson 3 4
                
        let jsonKeys = (Helpers.flatten (fun n -> n.Children) root) 
                        |> Seq.map (fun n -> n.Key)
                        |> Set.ofSeq
    
        let builtKeys = Hydrator.hydrateCategories json Map.empty 
                        |> Map.toSeq
                        |> Seq.map (fun kv -> match kv with (k, v) -> k)
                        |> Set.ofSeq

        builtKeys |> should equal jsonKeys


    [<Test>]
    member x.``articulates cat children in hierarchically correct manner`` () =
        let root, json = createCatJson 3 4

        let srcNodes = (Helpers.flatten (fun n -> n.Children) root)
                        
        let builtNodes = (Hydrator.hydrateCategories json Map.empty).["root"]
                         |> Helpers.flatten (fun n -> n.Children)

        srcNodes 
        |> Seq.zip builtNodes
        |> Seq.iter (fun (srcNode, builtNode) -> 
                            srcNode.Key |> should equal builtNode.Key

                            let srcChildKeys = srcNode.Children
                                                |> List.map (fun c -> c.Key)

                            let builtChildKeys = builtNode.Children
                                                    |> List.map (fun c -> c.Key)

                            builtChildKeys |> should equal srcChildKeys
                            )



    [<Test>]
    member x.``groups products per cat`` () =
        let root, json = createCatJson 3 4

        let catNodes = Helpers.flatten (fun n -> n.Children) root
                    
        let prods = catNodes 
                    |> Seq.collect (fun c -> [0..4] |> Seq.map (fun _ -> createProd [c.Key])) 
                    |> Seq.map (fun p -> (p.Key, p))
                    |> Map.ofSeq

        Hydrator.hydrateCategories json prods
        |> Map.iter (fun k c -> c.Products.Length |> should equal 5)
