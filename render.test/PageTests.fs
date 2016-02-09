module Pages

open System
open NUnit.Framework
open FsUnit

open BoditeRender
open BoditeRender.Pages



let createCatOfProds key prods =    
    {
        Key = key
        Name = { LV=None; RU=None }
        Description = { LV=None; RU=None }
        Children = []
        Products = prods
    }

let createProduct catKeys =
    {
        Key = Guid.NewGuid().ToString()
        Name = { LV=None; RU=None }
        Description = { LV=None; RU=None }
        MachineName = ""
        CategoryKeys = catKeys
    }
    

let ofType<'T> subject =
    box subject :? 'T


let catKeys = [1..5] |> List.map (fun _ -> Guid.NewGuid().ToString())
let prods = [1..30] |> List.map (fun _ -> createProduct (catKeys |> Helpers.getRandomSelection 2))
let cats = catKeys |> List.map (fun key -> createCatOfProds key (prods 
                                                                    |> List.filter (fun p -> p.CategoryKeys |> Seq.exists (fun k -> k.Equals(key)) )))

let model = new Model(products=prods, categories=cats);




[<TestFixture>]
type ``buildPages`` () =    

    [<Test>]
    member x.``returns list of pages`` () =
        let res = Pages.buildPages model
        res.GetType() |> should equal (List.empty<Page>.GetType())
    

    [<Test>]
    member x.``builds one ProductPage per Product * Category`` () =    
        model
        |> Pages.buildPages
        |> Seq.filter ofType<ProductPage>
        |> Seq.length |> should equal (model.Products |> Seq.collect (fun p -> p.CategoryKeys) |> Seq.length)


    [<Test>]
    member x.``builds one page per Category`` () = 
        model
        |> Pages.buildPages
        |> Seq.filter ofType<CategoryPage>
        |> Seq.length |> should equal (model.Categories |> Seq.length)


    [<Test>]
    member x.``builds one unique HomePage`` () =
        new Model()
        |> Pages.buildPages
        |> Seq.filter ofType<HomePage>
        |> Seq.length |> should equal 1


