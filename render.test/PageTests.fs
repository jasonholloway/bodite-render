module Pages

open System
open NUnit.Framework
open FsUnit

open BoditeRender
open BoditeRender.Pages



let createCatOfProds key prods =    
    {
        Key = key
        Name = LocaleString []
        Description = LocaleString []
        Children = []
        Products = prods
    }

let createProduct catKeys =
    {
        Key = Guid.NewGuid().ToString()
        Name = LocaleString []
        Description = LocaleString []
//        MachineName = ""
        CategoryKeys = catKeys
    }
    

let ofType<'T> subject =
    box subject :? 'T


let catKeys = [1..5] |> List.map (fun _ -> Guid.NewGuid().ToString())

let prods = [1..30] 
            |> Seq.map (fun _ -> createProduct (catKeys |> Helpers.getRandomSelection 2)) 
            |> Seq.map (fun p -> (p.Key, p))
            |> Map.ofSeq 

let cats =  catKeys 
            |> Seq.map 
                (fun key -> createCatOfProds key (prods 
                                                    |> Map.toSeq
                                                    |> Seq.filter (fun (k, p) -> p.CategoryKeys |> Seq.exists (fun ck -> ck.Equals(key)))
                                                    |> Seq.map (fun (k, p) -> p)
                                                    |> Seq.toList
                                                    ))
            |> Seq.map (fun c -> (c.Key, c))
            |> Map.ofSeq

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
        |> Seq.length |> should equal (model.Products 
                                            |> Map.toSeq 
                                            |> Seq.collect (fun (_, p) -> p.CategoryKeys) 
                                            |> Seq.length)


    [<Test>]
    member x.``builds CategoryPage per Category * Locale`` () = 
        model
        |> Pages.buildPages
        |> Seq.filter ofType<CategoryPage>
        |> Seq.length |> should equal model.Categories.Count



    [<Test>]
    member x.``builds one unique HomePage`` () =
        new Model()
        |> Pages.buildPages
        |> Seq.filter ofType<HomePage>
        |> Seq.length |> should equal 1


