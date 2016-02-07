module Paths

open System
open NUnit.Framework
open FsUnit
open FSharp.Data

open BoditeRender
open BoditeRender.Paths


let createCat () =
    {
        Key = System.Guid.NewGuid().ToString()
        Name = { LV = None; RU = None }
        Description = { LV = None; RU = None }
        Children = []
        Products = []
    }

let createProd () =
    {
        Key = System.Guid.NewGuid().ToString()
        Name = { LV = None; RU = None }
        Description = { LV = None; RU = None }
        MachineName = "kjhkjhuhu"
        CategoryKeys = []
    }
    


[<TestFixture>]
type ``resolvePath`` () =

    [<Test>]
    member x.``resolves Product`` () =
        let prod = createProd()

        Paths.resolvePath prod
        |> should equal ("product/" + prod.MachineName)


    [<Test>]
    member x.``resolves Category`` () =
        let cat = createCat()

        Paths.resolvePath cat
        |> should equal ("category/" + cat.Key)