module Render

open NUnit.Framework
open FsUnit
open System
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
        



[<TestFixture>]
type ``renderPage`` () =

    [<Test>]
    member x.``returns VirtFile list`` () =
        Render.renderPage (new Page(path="", title=None))
        |> should be ofExactType<VirtFile list>
         
         
    [<Test>]
    member x.``renders ProductPage to single HTML file`` () =
        let cat, prod = createCatAndProd()
        let files = new ProductPage(prod=prod, cat=cat)
                    |> Render.renderPage
        
        files.Length |> should equal 1

        let file = files.[0]

        file.Data.Length |> should be (greaterThan 0)

        //should also test that is HTML file by running regex on text contents

