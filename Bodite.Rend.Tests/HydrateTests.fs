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


let createDbCat k childKeys =
    {
        DbCategory.Key = k
        Name = Map.empty
        ChildKeys = childKeys
    }


let createDbCats branching depth =
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
                 |> Seq.map (fun n -> createDbCat n.Key (n.Children
                                                         |> List.map (fun c -> c.Key))) 
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
    
let private createProduct catKeys =
    {
        Product.Key = System.Guid.NewGuid().ToString()
        Name = LocaleString []
        Description = LocaleString []
        CategoryKeys = catKeys
    }

let private createCat () =
    {
        Category.Key = System.Guid.NewGuid().ToString()
        Name = LocaleString []
        Description = LocaleString []
        Children = []
        Products = []
    }




[<TestFixture>]
type ``hydrateModel`` () =
    
    [<Test>]
    member x.``Exposes map of cats`` () =    
        let dbModel = DbModel(categories=([0..50] |> List.map (fun i -> createDbCat (i.ToString()) [])))

        let model = dbModel |> Hydrate.hydrateModel

        model.Categories
        |> Seq.map (fun c -> c.Key)
        |> Set.ofSeq
        |> should equal (dbModel.Categories
                         |> Seq.map (fun c -> c.Key)
                         |> Set.ofSeq)



        
    [<Test>]
    member x.``Exposes map of prods`` () =
        let dbModel = DbModel(products=( [0..50] |> List.map (fun i -> createDbProduct([])) ))

        let model = dbModel |> Hydrate.hydrateModel

        model.Products
        |> Seq.map (fun p -> p.Key)
        |> Set.ofSeq
        |> should equal (dbModel.Products
                         |> Seq.map (fun p -> p.Key)
                         |> Set.ofSeq)





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
        
        


[<TestFixture>]
type ``hydrateCategories`` () =
    [<Test>]
    member x.``emits entire tree of cats`` () =
        let _, dbCats = createDbCats 3 4
                
        let dbKeys = dbCats 
                     |> List.map (fun c -> c.Key)
                     |> Set.ofSeq
    
        let builtKeys = dbCats
                        |> Hydrate.hydrateCategories Set.empty
                        |> Seq.map (fun c -> c.Key)
                        |> Set.ofSeq

        builtKeys |> should equal dbKeys





    [<Test>]
    member x.``groups products per cat`` () =
        let _, dbCats = createDbCats 3 4
            
        let prods = dbCats 
                        |> Seq.collect (fun c -> [0..4] |> Seq.map (fun _ -> createProduct [c.Key])) 
                        |> Set.ofSeq

        dbCats
        |> Hydrate.hydrateCategories prods
        |> List.iter (fun c -> c.Products.Length |> should equal 5)



    [<Test>]
    member x.``articulates cat children in hierarchically correct manner`` () =
        let rootSpec, dbCats = createDbCats 3 4

        let specs = (Helpers.flatten (fun n -> n.Children) rootSpec)
                        
        let cats = dbCats
                   |> Hydrate.hydrateCategories Set.empty
                   |> Seq.find (fun c -> c.Key.Equals("root"))
                   |> Helpers.flatten (fun n -> n.Children)

        specs
        |> Seq.zip cats
        |> Seq.iter (fun (spec, cat) -> 
                            spec.Key |> should equal cat.Key

                            let specChildKeys = spec.Children
                                                |> List.map (fun c -> c.Key)

                            let catChildKeys = cat.Children
                                                    |> List.map (fun c -> c.Key)

                            catChildKeys |> should equal specChildKeys
                            )
//
//    [<Test>]
//    member x.``handles empty children elegantly`` () =
//        let dbCats = [{
//                        DbCategory.Key = Guid.NewGuid().ToString()
//                        Name = Map.empty
//                        Children = None
//                      }]
//
//        let cats = dbCats |> Hydrate.hydrateCategories Map.empty
//
//        cats.Length |> should equal 1
//        cats.Head.Children |> should equal []