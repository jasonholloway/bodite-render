module Hydrate

open System
open NUnit.Framework
open FsUnit
open FSharp.Data

open BoditeRender
open Hydrate
open Json


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

    let dbCats = Helpers.flatten (fun n -> n.Children) rootNode
                 |> Seq.map (fun n -> {
                                        DbCategory.Key = n.Key
                                        Name = Map.empty
                                        ChildKeys = n.Children
                                                    |> List.map (fun c -> c.Key)
                                        }) 
                 |> Seq.toList
                     
    ( rootNode, dbCats )

    

let private createDbProduct catKeys =
    {
        DbProduct.Key = System.Guid.NewGuid().ToString()
        Name = Map.empty
        Description = Map.empty
        //MachineName = ""
        CategoryKeys = catKeys
    }
    




[<TestFixture>]
type ``hydrateLocaleString`` () = 
    
    [<Test>]
    member x.``suppresses unrecognised locale codes, while exposing proper ones`` () =
        let l = Locales.All.Head

        let m = Map<string, string>([
                                        (l.Code, "Blah")
                                        ("ZZ", "ARGH")
                                    ])  

        let s = Hydrate.hydrateLocaleString m

        s.get(l).IsSome |> should equal true



[<TestFixture>]
type ``hydrateProducts`` () =
    
    [<Test>]
    member x.``returns one Product per DbProduct, with keys`` () =
        let dbProds = [0..20]
                      |> List.map (fun i -> createDbProduct [])

        let prods = Hydrate.hydrateProducts dbProds

        dbProds 
        |> List.map (fun p -> p.Key) 
        |> Set.ofList
        |> should equal (prods 
                         |> List.map (fun p -> p.Key) 
                         |> Set.ofList)
        
        





//[<TestFixture>]
//type ``hydrateCategories`` () =
//    [<Test>]
//    member x.``includes all cats in map`` () =
//        let root, json = createCatJson 3 4
//                
//        let jsonKeys = (Helpers.flatten (fun n -> n.Children) root) 
//                        |> Seq.map (fun n -> n.Key)
//                        |> Set.ofSeq
//    
//        let builtKeys = Hydrate.hydrateCategories json Map.empty 
//                        |> Map.toSeq
//                        |> Seq.map (fun kv -> match kv with (k, v) -> k)
//                        |> Set.ofSeq
//
//        builtKeys |> should equal jsonKeys
//
//
//    [<Test>]
//    member x.``articulates cat children in hierarchically correct manner`` () =
//        let root, json = createCatJson 3 4
//
//        let srcNodes = (Helpers.flatten (fun n -> n.Children) root)
//                        
//        let builtNodes = (Hydrate.hydrateCategories json Map.empty).["root"]
//                         |> Helpers.flatten (fun n -> n.Children)
//
//        srcNodes 
//        |> Seq.zip builtNodes
//        |> Seq.iter (fun (srcNode, builtNode) -> 
//                            srcNode.Key |> should equal builtNode.Key
//
//                            let srcChildKeys = srcNode.Children
//                                                |> List.map (fun c -> c.Key)
//
//                            let builtChildKeys = builtNode.Children
//                                                    |> List.map (fun c -> c.Key)
//
//                            builtChildKeys |> should equal srcChildKeys
//                            )
//
//
//
//    [<Test>]
//    member x.``groups products per cat`` () =
//        let root, json = createCatJson 3 4
//
//        let catNodes = Helpers.flatten (fun n -> n.Children) root
//                    
//        let prods = catNodes 
//                    |> Seq.collect (fun c -> [0..4] |> Seq.map (fun _ -> createProd [c.Key])) 
//                    |> Seq.map (fun p -> (p.Key, p))
//                    |> Map.ofSeq
//
//        Hydrate.hydrateCategories json prods
//        |> Map.iter (fun k c -> c.Products.Length |> should equal 5)
