module Render

open NUnit.Framework
open FsUnit
open FsUnit.TopLevelOperators
open System
open System.IO
open BoditeRender


let createCategory key prods =    
    {
        Key = key
        Name = LocaleString []
        Description = LocaleString []
        Children = []
        Products = prods
    }

let createProduct catKeys =
    {
        Product.Key = Guid.NewGuid().ToString()
        Name = LocaleString []
        Description = LocaleString []
//        MachineName = ""
        CategoryKeys = catKeys
    }
    
let createCatAndProd () =
    let catKey = Guid.NewGuid().ToString()
    let prod = createProduct [catKey]
    let cat = createCategory catKey [prod]
    (cat, prod)
        


type TestPage () =
    inherit Page("Test")

    override Page.Path = ""
    override Page.Title = "Hello"





[<TestFixture>]
type ``renderPage`` () =

    let ctx = RenderContext(Model(), (fun r -> None))


    [<Test>]
    member x.``returns VirtFile list`` () =
        let renderer = Renderer((fun s -> "hello!"), ctx)

        renderer.renderPage (TestPage())
        |> should be ofExactType<VirtFile list>
         
    
    [<Test>]
    member x.``resolves template with typename + 'cshtml' of passed Page`` () =
        let renderer = Renderer((fun k -> 
                                    match k with
                                    | "TestPage.cshtml"    -> "hello!" 
                                    | _         -> failwith "Bad template key!"
                                    ),
                                    ctx)
                                    
        let result = renderer.renderPage (TestPage())
        
        result |> should be ofExactType<VirtFile list>
        result |> should haveLength 1

        result.Head.Data
        |> Helpers.stream2String
        |> should equal "hello!"


