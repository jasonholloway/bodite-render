﻿module Paths

open System
open NUnit.Framework
open FsUnit
open FSharp.Data

open BoditeRender
open BoditeRender.Paths


let createCat () =
    {
        Key = System.Guid.NewGuid().ToString()
        Name = LocaleString []
        Description = LocaleString []
        Children = []
        Products = []
    }

let createProd () =
    {
        Product.Key = System.Guid.NewGuid().ToString()
        Name = LocaleString []
        Description = LocaleString []
//        MachineName = "kjhkjhuhu"
        CategoryKeys = []
    }
    


[<TestFixture>]
type ``resolvePath`` () =

    [<Test>]
    member x.``resolves Product`` () =
        let prod = createProd()

        Paths.resolvePath prod
        |> should equal ("product/" + "") // prod.MachineName)


    [<Test>]
    member x.``resolves Category`` () =
        let cat = createCat()

        Paths.resolvePath cat
        |> should equal ("category/" + cat.Key)