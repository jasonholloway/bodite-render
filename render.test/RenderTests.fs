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
    
let createCatAndProd () =
    let catKey = Guid.NewGuid().ToString()
    let prod = createProduct [catKey]
    let cat = createCategory catKey [prod]
    (cat, prod)
        

let stream2String (str: Stream) =
    using (new StreamReader(str))
            (fun r -> r.ReadToEnd())



[<TestFixture>]
type ``renderPage`` () =

    [<Test>]
    member x.``returns VirtFile list`` () =
        let renderer = Renderer(fun s -> "hello!")

        renderer.renderPage (new Page(path="", title=None))
        |> should be ofExactType<VirtFile list>
         
    
    [<Test>]
    member x.``calls template resolver with typename of passed Page`` () =
        let renderer = Renderer(fun k -> 
                                    match k with
                                    | "Page"    -> "hello!" 
                                    | _         -> failwith "Bad template key!")
                                    
        let result = renderer.renderPage (new Page(path="", title=None))
        
        result |> should be ofExactType<VirtFile list>
        result |> should haveLength 1

        using (new StreamReader(result.Head.Data))
                (fun r -> 
                    r.ReadToEnd() |> should equal "hello!"
                )




//         
//    [<Test>]
//    member x.``renders ProductPage to single HTML file`` () =
//        let cat, prod = createCatAndProd()
//        let files = new ProductPage(prod=prod, cat=cat)
//                    |> Render.renderPage
//        
//        files.Length |> should equal 1
//
//        let file = files.[0]
//
//        file.Data.Length |> should be (greaterThan 0)
//        
//        (file.Data |> stream2String).ToLower().Contains("<html>")
//        |> should equal true
//                
